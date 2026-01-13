import os
import shutil
from pathlib import Path

def copiar_strings_sims():
    # Configuración de rutas
    origen_base = Path(r"D:\SteamLibrary\steamapps\common\The Sims 4")
    destino_base = Path(r"C:\Users\user\Desktop\Strings")

    # Definimos las subcarpetas de interés
    subcarpetas_objetivo = ["Data/Client", "Delta"]

    print("--- Iniciando copia de archivos .package ---")

    for sub in subcarpetas_objetivo:
        ruta_busqueda = origen_base / sub
        
        # Verificamos si la carpeta de origen existe
        if not ruta_busqueda.exists():
            print(f"⚠️ No se encontró la carpeta: {ruta_busqueda}")
            continue

        # Buscamos recursivamente (rglob) archivos que empiecen con 'Strings_' y terminen en '.package'
        # Usamos .lower() para que no importe si dice 'strings' o 'Strings'
        for archivo in ruta_busqueda.rglob("*"):
            if archivo.name.lower().startswith("strings_") and archivo.suffix.lower() == ".package":
                
                # Calculamos la ruta relativa para replicar la estructura
                # Ejemplo: Data/Client/Strings_ES_ES.package
                ruta_relativa = archivo.relative_to(origen_base)
                ruta_destino_final = destino_base / ruta_relativa

                # Creamos las carpetas necesarias en el destino si no existen
                ruta_destino_final.parent.mkdir(parents=True, exist_ok=True)

                # Copiamos el archivo
                try:
                    shutil.copy2(archivo, ruta_destino_final)
                    print(f"✅ Copiado: {ruta_relativa}")
                except Exception as e:
                    print(f"❌ Error al copiar {archivo.name}: {e}")

    print("\n--- Proceso finalizado con éxito ---")
    print(f"Los archivos están en: {destino_base}")

if __name__ == "__main__":
    copiar_strings_sims()