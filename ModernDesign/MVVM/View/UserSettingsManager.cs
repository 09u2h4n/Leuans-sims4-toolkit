using System;
using System.IO;

namespace ModernDesign.Profile
{
    public static class UserSettingsManager
    {
        private static readonly string AppDataFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Leuan's - Sims 4 ToolKit"
        );

        private static readonly string SettingsFilePath = Path.Combine(AppDataFolder, "settingsp.ini");

        static UserSettingsManager()
        {

            // Verificar si el archivo existe (incluso si está oculto)
            if (!FileExistsIncludingHidden(SettingsFilePath))
            {
                SaveBackgroundColors("#22D3EE", "#1E293B", "#21b96b");
                SaveAvatar("👤");
            }
        }

        private static bool FileExistsIncludingHidden(string path)
        {
            try
            {
                // Esto funciona incluso con archivos ocultos
                return File.Exists(path) || new FileInfo(path).Exists;
            }
            catch
            {
                return false;
            }
        }


        public static void SaveBackgroundColors(string color1, string color2, string color3)
        {
            try
            {
                // Quitar atributo oculto temporalmente si existe
                if (File.Exists(SettingsFilePath))
                {
                    File.SetAttributes(SettingsFilePath, FileAttributes.Normal);
                }

                var lines = File.Exists(SettingsFilePath) ? File.ReadAllLines(SettingsFilePath) : new string[0];

                using (StreamWriter writer = new StreamWriter(SettingsFilePath))
                {
                    bool bg1Written = false, bg2Written = false, bg3Written = false;

                    foreach (var line in lines)
                    {
                        if (line.StartsWith("background1="))
                        {
                            writer.WriteLine($"background1={color1}");
                            bg1Written = true;
                        }
                        else if (line.StartsWith("background2="))
                        {
                            writer.WriteLine($"background2={color2}");
                            bg2Written = true;
                        }
                        else if (line.StartsWith("background3="))
                        {
                            writer.WriteLine($"background3={color3}");
                            bg3Written = true;
                        }
                        else
                        {
                            writer.WriteLine(line);
                        }
                    }

                    if (!bg1Written) writer.WriteLine($"background1={color1}");
                    if (!bg2Written) writer.WriteLine($"background2={color2}");
                    if (!bg3Written) writer.WriteLine($"background3={color3}");
                }

                // Volver a poner como oculto
                File.SetAttributes(SettingsFilePath, FileAttributes.Hidden);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error saving background colors: {ex.Message}", ex);
            }
        }

        public static void SaveAvatar(string avatar)
        {
            try
            {
                // Quitar atributo oculto temporalmente si existe
                if (File.Exists(SettingsFilePath))
                {
                    File.SetAttributes(SettingsFilePath, FileAttributes.Normal);
                }

                var lines = File.Exists(SettingsFilePath) ? File.ReadAllLines(SettingsFilePath) : new string[0];

                using (StreamWriter writer = new StreamWriter(SettingsFilePath))
                {
                    bool avatarWritten = false;

                    foreach (var line in lines)
                    {
                        if (line.StartsWith("avatar="))
                        {
                            writer.WriteLine($"avatar={avatar}");
                            avatarWritten = true;
                        }
                        else
                        {
                            writer.WriteLine(line);
                        }
                    }

                    if (!avatarWritten) writer.WriteLine($"avatar={avatar}");
                }

                // Volver a poner como oculto
                File.SetAttributes(SettingsFilePath, FileAttributes.Hidden);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error saving avatar: {ex.Message}", ex);
            }
        }

        public static string[] GetBackgroundColors()
        {
            try
            {
                if (!File.Exists(SettingsFilePath))
                    return new string[] { "#22D3EE", "#1E293B", "#21b96b" };

                // Quitar atributo oculto temporalmente para leer
                var originalAttributes = File.GetAttributes(SettingsFilePath);
                if ((originalAttributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                {
                    File.SetAttributes(SettingsFilePath, FileAttributes.Normal);
                }

                string color1 = "#22D3EE", color2 = "#1E293B", color3 = "#21b96b";

                foreach (var line in File.ReadAllLines(SettingsFilePath))
                {
                    if (line.StartsWith("background1="))
                        color1 = line.Substring("background1=".Length).Trim();
                    else if (line.StartsWith("background2="))
                        color2 = line.Substring("background2=".Length).Trim();
                    else if (line.StartsWith("background3="))
                        color3 = line.Substring("background3=".Length).Trim();
                }

                // Restaurar atributo oculto
                File.SetAttributes(SettingsFilePath, originalAttributes);

                return new string[] { color1, color2, color3 };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error reading background colors: {ex.Message}", ex);
            }
        }


        public static string GetAvatar()
        {
            try
            {
                if (!File.Exists(SettingsFilePath))
                    return "👤";

                // Quitar atributo oculto temporalmente para leer
                var originalAttributes = File.GetAttributes(SettingsFilePath);
                if ((originalAttributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                {
                    File.SetAttributes(SettingsFilePath, FileAttributes.Normal);
                }

                string avatar = "👤";

                foreach (var line in File.ReadAllLines(SettingsFilePath))
                {
                    if (line.StartsWith("avatar="))
                    {
                        avatar = line.Substring("avatar=".Length).Trim();
                        break;
                    }
                }

                // Restaurar atributo oculto
                File.SetAttributes(SettingsFilePath, originalAttributes);

                return avatar;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error reading avatar: {ex.Message}", ex);
            }
        }

        // Método para verificar si el archivo existe y está accesible
        public static bool IsSettingsFileValid()
        {
            try
            {
                return File.Exists(SettingsFilePath) && File.ReadAllText(SettingsFilePath).Length > 0;
            }
            catch
            {
                return false;
            }
        }
    }
}