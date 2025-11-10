
# 🎵 Musical Recommender System

Un sistema inteligente de recomendación musical que genera playlists personalizadas basadas en el análisis de relaciones entre artistas y algoritmos de búsqueda tabú.

## 🌟 Características Principales

-   **Generación Automática de Playlists**: Crea listas de reproducción coherentes basadas en una canción inicial
-   **Análisis de Relaciones entre Artistas**: Utiliza LastFM y Spotify para construir un grafo de artistas similares
-   **Algoritmo de Búsqueda Tabú**: Balancea familiaridad y diversidad musical
-   **Integración con Spotify**: Exporta playlists directamente a tu cuenta
-   **Tiempo Real**: Actualizaciones en vivo del progreso mediante SignalR

## 🧠 Arquitectura del Algoritmo

### 1. Campo Generativo Musical (`MusicalGenerativeField`)

El primer paso construye un "campo generativo" de artistas relacionados:

**Proceso:**

-   Extrae el artista de la canción inicial
-   Consulta artistas similares en LastFM
-   Expande recursivamente hasta obtener ~20 artistas
-   Prioriza relevancia (primeros resultados) con aleatoriedad controlada

### 2. Filtrado Contextual (`TreeFlattener`)

Convierte el árbol de artistas en una lista plana ordenada:

```
BFS (Breadth-First Search)
├── Artista Raíz (mayor familiaridad)
├── Artistas de Nivel 1 (familiaridad media)
└── Artistas de Nivel 2 (mayor diversidad)

```

### 3. Recolección de Canciones (`GetTracksFromArtists`)

Para cada artista en el grafo:

-   Obtiene sus álbumes (singles + álbumes completos)
-   Ordena por popularidad
-   Extrae canciones de los top 5 álbumes
-   Genera una muestra de ~500-1000 canciones

### 4. Búsqueda Tabú (`TabuSearch`)

El corazón del sistema. Implementa un algoritmo de optimización que:

#### **Métricas de Diversidad**

Calcula un índice Herfindahl-Hirschman modificado:

```
Diversidad = Σ(frecuencia_artista²) / total_canciones²

```

-   **≤ 0.45**: Modo "Familiaridad" (prioriza artistas conocidos)
-   **> 0.45**: Modo "Diversidad" (introduce artistas nuevos)

#### **Lista Tabú**

Previene la repetición excesiva:

-   Artistas con >5 apariciones → Bloqueados 20 iteraciones
-   En modo diversidad → Bloqueados 30 iteraciones
-   Canciones agotadas → Bloqueados permanentemente

### 5. Generación de Cola Final (`GeneratePlayQueue`)

Organiza las canciones seleccionadas:

1.  **Calcula tamaño**: `duración_total / 3 minutos`
2.  **Distribución**:
    -   70% posiciones de familiaridad (F)
    -   30% posiciones de diversidad (D)
3.  **Asignación**:
    -   Canción raíz en posición [0]
    -   Artistas diversos en posiciones aleatorias marcadas como D
    -   Artistas familiares distribuidos en posiciones F (40% de representación inicial por artista)

```
Playlist Final:
[Raíz] [F] [D] [F] [F] [D] [F] [D] [F] [F] ...
```

## 🚀 Endpoints Principales

### `POST /api/PlaylistGenerator`

Genera una playlist completa

**Request Body:**

```json
{
  "roots": [
    {
      "key": "spotify_track_id",
      "duration": 60
    }
  ],
  "connectionID": "signalr_connection_id"
}

```

**Response:**

```json
[
  {
    "id": "track_id",
    "name": "Track Name",
    "artists": [...],
    "album": {...},
    "duration_ms": 180000,
    "popularity": 85
  }
]

```

### `POST /api/getPlaylistUrl`

Exporta a Spotify

**Request Body:**

```json
{
  "playlist": [
    "spotify:track:xxx",
    "spotify:track:yyy"
  ]
}

```

**Response:**

```json
{
  "playlistUrl": "https://open.spotify.com/playlist/..."
}

```

### `GET /api/SpotifyAuthServiceController`

Obtiene el token de autenticación

**Response:**

```json
{
  "request_token": "BQC..."
}

```

### `GET /api/SpotifyAuthServiceController/Callback`

Callback de OAuth de Spotify

**Query Parameters:**

-   `code`: Authorization code de Spotify

## 🛠️ Tecnologías

-   **ASP.NET Core 8.0**
-   **SignalR**: Comunicación en tiempo real
-   **Spotify Web API**: Datos musicales y creación de playlists
-   **LastFM API**: Relaciones entre artistas
-   **Docker**: Containerización

## 📊 Ventajas del Algoritmo

1.  **Balance Dinámico**: Adapta familiaridad/diversidad según el progreso
2.  **Evita Repetición**: Sistema de tabú sofisticado
3.  **Contextual**: Respeta las relaciones musicales reales
4.  **Escalable**: Procesamiento concurrente con semáforos
5.  **Personalizable**: Duración y puntos de inicio configurables

## 🎯 Casos de Uso

-   **Discovery Musical**: Descubre artistas similares manteniendo tu zona de confort
-   **Playlists de Trabajo**: Música coherente sin interrupciones bruscas
-   **Exploración de Géneros**: Transiciones suaves entre estilos relacionados
-   **Sesiones DJ**: Flujo natural entre artistas

## 🐳 Docker

### Build

```bash
docker build -t musical-recommender .
```

### Run

```bash
docker run -p 5000:5000 \
  -e SPOTIFY_CLIENT_ID=your_id \
  -e SPOTIFY_CLIENT_SECRET=your_secret \
  -e LASTFM_API_KEY=your_key \
  musical-recommender

## 📡 SignalR Hub

### Eventos del Cliente

-   **`getFeedback`**: Recibe el grafo de artistas generado
-   **`updateSongsFound`**: Actualización del conteo de canciones encontradas

### Conexión

```javascript
const connection = new signalR.HubConnectionBuilder()
    .withUrl("http://localhost:5000/progresshub")
    .build();

connection.on("getFeedback", (artists) => {
    console.log("Artistas:", artists);
});

connection.on("updateSongsFound", (count) => {
    console.log("Canciones encontradas:", count);
});

await connection.start();

```

## 🧪 Ejemplo de Uso

```csharp
// Crear una playlist de 60 minutos basada en una canción
var request = new List<Station> {
    new Station {
        key = "3n3Ppam7vgaVa1iaRUc9Lp", // Mr. Brightside - The Killers
        duration = 60
    }
};

var playlist = await playlistGenerator.GetPlaylist(request, connectionId);

// Exportar a Spotify
var trackUris = playlist.Select(t => t.uri).ToList();
var playlistUrl = await playlistGenerator.getPlaylistURL(trackUris);

```

## 🎓 Conceptos Clave

### Lista Tabú

Estructura que previene ciclos en la búsqueda:

```csharp
Dictionary<string, int> tabuList;
// Key: Artist ID
// Value: Iteración hasta la cual está bloqueado

```

### BFS (Breadth-First Search)

Garantiza que artistas más relacionados aparezcan primero en el grafo:

```
Nivel 0: Artista raíz
Nivel 1: Artistas similares directos
Nivel 2: Artistas similares a los similares

```

## 📈 Métricas de Performance

-   **Tiempo de generación**: ~30-60 segundos para playlist de 60 minutos
-   **Canciones procesadas**: ~500-1000 por ejecución
-   **Artistas analizados**: ~20 por generación
-   **Rate limiting**: 2 requests concurrentes a Spotify API
