# 🎮 Sims 4 Strings Package Copier

A Python utility to extract and organize language string files from The Sims 4 installation.

## What does this do?

If you've ever needed to work with The Sims 4 localization files, you know they're scattered across different folders in your game installation. This script finds all the `Strings_*.package` files and copies them to a single organized location while preserving the original folder structure.

Perfect for modders, translators, or anyone curious about how the game handles different languages.

## Features

- Automatically locates string packages in your Sims 4 installation
- Preserves original directory structure
- Searches recursively through Data/Client and Delta folders
- Case-insensitive file matching
- Progress feedback as files are copied

## Requirements

- Python 3.6 or higher
- The Sims 4 installed on your system
- No external dependencies needed (uses only Python standard library)

## How to use

1. **Clone or download** this repository

2. **Edit the paths** in the script to match your system:
   ```python
   origen_base = Path(r"D:\SteamLibrary\steamapps\common\The Sims 4")
   destino_base = Path(r"C:\Users\user\Desktop\Strings")
   ```

3. **Run the script:**
   ```bash
   python copiar_strings_sims.py
   ```

The script will create a `Strings` folder (or whatever destination you chose) containing all the language package files organized by their original location.

## Configuration

You can modify which folders get searched by editing the `subcarpetas_objetivo` list:

```python
subcarpetas_objetivo = ["Data/Client", "Delta"]
```

Add or remove folders as needed for your specific use case.

## Output example

```
✅ Copiado: Data/Client/Strings_ENG_US.package
✅ Copiado: Data/Client/Strings_ESP_ES.package
✅ Copiado: Delta/EP01/Strings_ENG_US.package
⚠️ No se encontró la carpeta: Delta/SomeOtherPack

Proceso finalizado con éxito
Los archivos están en: C:\Users\user\Desktop\Strings
```

## Why would I need this?

- **Mod development:** Extract strings to analyze or create translations
- **Language research:** Compare how different languages handle game text
- **Backup purposes:** Keep a clean copy of all language files
- **Translation work:** Easy access to all localization packages

## License

Free to use and modify as you wish. If you make something cool with it, feel free to share!

## Contributing

Found a bug or have a suggestion? Open an issue or submit a pull request. All contributions are welcome.

---

# 🎮 Copiador de Archivos Strings de Los Sims 4

Una utilidad en Python para extraer y organizar los archivos de idioma de tu instalación de Los Sims 4.

## ¿Qué hace esto?

Si alguna vez has necesitado trabajar con los archivos de localización de Los Sims 4, sabes que están dispersos por diferentes carpetas en la instalación del juego. Este script encuentra todos los archivos `Strings_*.package` y los copia a una ubicación organizada manteniendo la estructura de carpetas original.

Perfecto para modders, traductores o cualquier persona curiosa sobre cómo el juego maneja los diferentes idiomas.

## Características

- Localiza automáticamente los paquetes de strings en tu instalación de Sims 4
- Preserva la estructura de directorios original
- Busca recursivamente en las carpetas Data/Client y Delta
- Coincidencia de archivos sin importar mayúsculas/minúsculas
- Retroalimentación del progreso mientras se copian los archivos

## Requisitos

- Python 3.6 o superior
- Los Sims 4 instalado en tu sistema
- No necesita dependencias externas (usa solo la librería estándar de Python)

## Cómo usar

1. **Clona o descarga** este repositorio

2. **Edita las rutas** en el script para que coincidan con tu sistema:
   ```python
   origen_base = Path(r"D:\SteamLibrary\steamapps\common\The Sims 4")
   destino_base = Path(r"C:\Users\user\Desktop\Strings")
   ```

3. **Ejecuta el script:**
   ```bash
   python copiar_strings_sims.py
   ```

El script creará una carpeta `Strings` (o el destino que hayas elegido) conteniendo todos los archivos de idioma organizados según su ubicación original.

## Configuración

Puedes modificar qué carpetas se buscan editando la lista `subcarpetas_objetivo`:

```python
subcarpetas_objetivo = ["Data/Client", "Delta"]
```

Agrega o elimina carpetas según tus necesidades específicas.

## Ejemplo de salida

```
✅ Copiado: Data/Client/Strings_ENG_US.package
✅ Copiado: Data/Client/Strings_ESP_ES.package
✅ Copiado: Delta/EP01/Strings_ENG_US.package
⚠️ No se encontró la carpeta: Delta/SomeOtherPack

Proceso finalizado con éxito
Los archivos están en: C:\Users\user\Desktop\Strings
```

## ¿Por qué necesitaría esto?

- **Desarrollo de mods:** Extrae strings para analizar o crear traducciones
- **Investigación lingüística:** Compara cómo diferentes idiomas manejan el texto del juego
- **Respaldo:** Mantén una copia limpia de todos los archivos de idioma
- **Trabajo de traducción:** Acceso fácil a todos los paquetes de localización

## Licencia

Libre para usar y modificar como desees. Si haces algo genial con esto, ¡compártelo!

## Contribuciones

¿Encontraste un bug o tienes una sugerencia? Abre un issue o envía un pull request. Todas las contribuciones son bienvenidas.
