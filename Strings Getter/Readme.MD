# 🎮 Sims 4 String Relocator

A modern desktop application to extract and organize language files from The Sims 4 installation. Built with Python and CustomTkinter for a sleek, user-friendly experience.

![Python](https://img.shields.io/badge/python-3.8+-blue.svg)
![CustomTkinter](https://img.shields.io/badge/CustomTkinter-5.0+-green.svg)
![License](https://img.shields.io/badge/license-MIT-blue.svg)

## What is this?

The Sims 4 stores language files (strings) scattered across different directories. This tool helps modders, translators, and curious players extract all `Strings_*.package` files and organize them neatly by language code, preserving the original folder structure.

After extraction, the app automatically opens the output folder so you can dive right into your files.

## Features

- **Modern Dark UI**: Clean, aesthetic interface built with CustomTkinter
- **Bilingual Support**: Switch between English and Spanish on the fly
- **Smart Detection**: Automatically finds all available languages in your game
- **Organized Output**: Files are grouped by language code (EN_US, ES_ES, etc.)
- **Real-time Feedback**: Console output shows exactly what's happening
- **Auto-open Folder**: Opens destination folder when extraction completes

## Screenshots

The app features a dark, modern interface with:
- Large, clear input fields for game and output paths
- Real-time console logging
- Language switcher (EN/ES)
- One-click extraction button

## Requirements

- Python 3.8 or higher
- The Sims 4 installed on your system
- CustomTkinter library

## Installation

1. **Clone or download this repository**

2. **Install dependencies:**
   ```bash
   pip install customtkinter
   ```

3. **Run the application:**
   ```bash
   python sims4_string_relocator.py
   ```

## How to Use

1. **Launch the app** - Double-click the Python file or run it from terminal
2. **Set your game path** - Browse to your Sims 4 installation folder (usually `D:\SteamLibrary\steamapps\common\The Sims 4` or similar)
3. **Choose output location** - Select where you want the organized files saved
4. **Click "START EXTRACTION"** - The app will:
   - Scan for available languages
   - Copy all string packages
   - Organize them by language
   - Open the destination folder

## Output Structure

```
Strings/
├── ENG_US/
│   ├── Data/Client/Strings_ENG_US.package
│   └── Delta/EP01/Strings_ENG_US.package
├── ESP_ES/
│   ├── Data/Client/Strings_ESP_ES.package
│   └── Delta/EP01/Strings_ESP_ES.package
└── ...
```

Each language gets its own folder with the complete file hierarchy preserved.

## Use Cases

**For Modders:**
- Extract strings to analyze game text structure
- Create translations or language patches
- Debug localization issues

**For Translators:**
- Access all language files in one organized location
- Compare translations across languages
- Export strings for translation workflows

**For Players:**
- Backup language files
- Explore game content
- Learn how localization works in The Sims 4

## Troubleshooting

**"The game path does not exist" error:**
- Make sure you selected the root Sims 4 folder (contains `Data` and `Delta` folders)
- Try browsing manually instead of typing the path

**No languages detected:**
- Verify your game installation is complete
- Check that `Data/Client` contains `Strings_*.package` files

**App won't start:**
- Ensure CustomTkinter is installed: `pip install customtkinter`
- Check Python version: `python --version` (needs 3.8+)

## Customization

Edit these lines in the code to change default paths:

```python
self.path_origin = ctk.StringVar(value=r"D:\SteamLibrary\steamapps\common\The Sims 4")
self.path_dest = ctk.StringVar(value=str(Path.home() / "Desktop" / "Strings"))
```

Change default language from Spanish to English:

```python
self.current_lang = "EN"  # Change from "ES" to "EN"
```

## Technical Details

- **Framework**: CustomTkinter (modern tkinter wrapper)
- **Theme**: Dark mode with custom color palette
- **File Operations**: Uses `shutil` for safe file copying
- **Path Handling**: `pathlib` for cross-platform compatibility

## Contributing

Found a bug? Have a feature idea? Contributions are welcome!

- Open an issue to report bugs
- Submit pull requests for improvements
- Share feedback on what works and what doesn't

## License

Free to use, modify, and distribute. Do whatever you want with it!

---

# 🎮 Relocalizador de Strings - Los Sims 4

Una aplicación de escritorio moderna para extraer y organizar archivos de idioma de la instalación de Los Sims 4. Construida con Python y CustomTkinter para una experiencia elegante y fácil de usar.

## ¿Qué es esto?

Los Sims 4 almacena archivos de idioma (strings) dispersos en diferentes directorios. Esta herramienta ayuda a modders, traductores y jugadores curiosos a extraer todos los archivos `Strings_*.package` y organizarlos ordenadamente por código de idioma, preservando la estructura de carpetas original.

Después de la extracción, la app abre automáticamente la carpeta de destino para que puedas acceder directamente a tus archivos.

## Características

- **UI Moderna Oscura**: Interfaz limpia y estética construida con CustomTkinter
- **Soporte Bilingüe**: Cambia entre inglés y español al instante
- **Detección Inteligente**: Encuentra automáticamente todos los idiomas disponibles en tu juego
- **Salida Organizada**: Los archivos se agrupan por código de idioma (EN_US, ES_ES, etc.)
- **Retroalimentación en Tiempo Real**: La consola muestra exactamente qué está pasando
- **Auto-abrir Carpeta**: Abre la carpeta de destino cuando completa la extracción

## Requisitos

- Python 3.8 o superior
- Los Sims 4 instalado en tu sistema
- Librería CustomTkinter

## Instalación

1. **Clona o descarga este repositorio**

2. **Instala las dependencias:**
   ```bash
   pip install customtkinter
   ```

3. **Ejecuta la aplicación:**
   ```bash
   python sims4_string_relocator.py
   ```

## Cómo Usar

1. **Inicia la app** - Haz doble clic en el archivo Python o ejecútalo desde la terminal
2. **Configura la ruta del juego** - Navega hasta tu carpeta de instalación de Los Sims 4
3. **Elige la ubicación de salida** - Selecciona dónde quieres guardar los archivos organizados
4. **Haz clic en "INICIAR EXTRACCIÓN"** - La app:
   - Escaneará los idiomas disponibles
   - Copiará todos los paquetes de strings
   - Los organizará por idioma
   - Abrirá la carpeta de destino

## Estructura de Salida

```
Strings/
├── ENG_US/
│   ├── Data/Client/Strings_ENG_US.package
│   └── Delta/EP01/Strings_ENG_US.package
├── ESP_ES/
│   ├── Data/Client/Strings_ESP_ES.package
│   └── Delta/EP01/Strings_ESP_ES.package
└── ...
```

Cada idioma obtiene su propia carpeta con la jerarquía completa de archivos preservada.

## Casos de Uso

**Para Modders:**
- Extraer strings para analizar la estructura de texto del juego
- Crear traducciones o parches de idioma
- Depurar problemas de localización

**Para Traductores:**
- Acceder a todos los archivos de idioma en una ubicación organizada
- Comparar traducciones entre idiomas
- Exportar strings para flujos de trabajo de traducción

**Para Jugadores:**
- Respaldar archivos de idioma
- Explorar contenido del juego
- Aprender cómo funciona la localización en Los Sims 4

## Solución de Problemas

**Error "La ruta del juego no existe":**
- Asegúrate de seleccionar la carpeta raíz de Sims 4 (contiene las carpetas `Data` y `Delta`)
- Intenta navegar manualmente en lugar de escribir la ruta

**No se detectan idiomas:**
- Verifica que tu instalación del juego esté completa
- Comprueba que `Data/Client` contenga archivos `Strings_*.package`

**La app no inicia:**
- Asegúrate de que CustomTkinter esté instalado: `pip install customtkinter`
- Verifica la versión de Python: `python --version` (necesitas 3.8+)

## Personalización

Edita estas líneas en el código para cambiar las rutas predeterminadas:

```python
self.path_origin = ctk.StringVar(value=r"D:\SteamLibrary\steamapps\common\The Sims 4")
self.path_dest = ctk.StringVar(value=str(Path.home() / "Desktop" / "Strings"))
```

Cambiar idioma predeterminado de español a inglés:

```python
self.current_lang = "EN"  # Cambia de "ES" a "EN"
```

## Detalles Técnicos

- **Framework**: CustomTkinter (wrapper moderno de tkinter)
- **Tema**: Modo oscuro con paleta de colores personalizada
- **Operaciones de Archivo**: Usa `shutil` para copia segura de archivos
- **Manejo de Rutas**: `pathlib` para compatibilidad multiplataforma

## Contribuciones

¿Encontraste un bug? ¿Tienes una idea para una funcionalidad? ¡Las contribuciones son bienvenidas!

- Abre un issue para reportar bugs
- Envía pull requests para mejoras
- Comparte feedback sobre qué funciona y qué no

## Licencia

Libre para usar, modificar y distribuir. ¡Haz lo que quieras con él!
