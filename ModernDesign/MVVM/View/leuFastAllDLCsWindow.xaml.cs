using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace ModernDesign.MVVM.View
{
    public partial class leuFastAllDLCsWindow : Window
    {
        public leuFastAllDLCsWindow()
        {
            InitializeComponent();
            Loaded += leuFastAllDLCsWindow_Loaded;
        }

        private void leuFastAllDLCsWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ApplyLanguage();
        }

        private static bool IsSpanishLanguage()
        {
            try
            {
                string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string languagePath = System.IO.Path.Combine(appData, "Leuan's - Sims 4 ToolKit", "language.ini");

                if (!System.IO.File.Exists(languagePath))
                    return false;

                var lines = System.IO.File.ReadAllLines(languagePath);
                foreach (var line in lines)
                {
                    var trimmed = line.Trim();
                    if (trimmed.StartsWith("Language") && trimmed.Contains("="))
                    {
                        var parts = trimmed.Split('=');
                        if (parts.Length == 2)
                        {
                            return parts[1].Trim().ToLower().Contains("es");
                        }
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        private void ApplyLanguage()
        {
            bool isSpanish = IsSpanishLanguage();

            if (isSpanish)
            {
                HeaderTitle.Text = "📦  Tutorial: Instalar Todos los DLCs";

                IntroTitle.Text = "Antes de Comenzar";
                IntroSubtitle.Text = "Elige tu método de descarga";
                IntroText.Text = "Primero debes descargar el paquete completo de DLCs. Elige uno de los siguientes métodos:";

                GDriveTitle.Text = "Google Drive";
                GDriveDesc.Text = "Descarga rápida y fácil";
                GDriveBtn.Content = "📥 Descargar desde Drive";

                TorrentTitle.Text = "BitTorrent";
                TorrentDesc.Text = "Más rápido para archivos grandes";
                TorrentBtn.Content = "🌐 Descargar Torrent";

                Step1Title.Text = "Extraer el Archivo Descargado";
                Step1Subtitle.Text = "Crea una carpeta nueva en tu Escritorio";
                Step1Text.Text = "Una vez descargado, extrae el archivo .zip a una carpeta NUEVA en tu Escritorio. Nómbrala algo como 'Sims4_DLCs'.";

                Step1Example.Text = "1. Haz clic derecho en el archivo .zip descargado\n" +
                                   "2. Selecciona 'Extraer todo...'\n" +
                                   "3. Elige tu Escritorio como destino\n" +
                                   "4. Crea una carpeta llamada 'Sims4_DLCs'";

                Step2Title.Text = "Usar el Instalador Semi-Automático";
                Step2Subtitle.Text = "Deja que la herramienta haga el trabajo";
                Step2Text.Text = "Ahora usa nuestro Instalador Semi-Automático para completar la instalación:";

                Step2Instruction1.Text = "Paso 1: Haz clic en 'Explorar' y selecciona la carpeta que acabas de extraer (Sims4_DLCs)";
                Step2Instruction2.Text = "Paso 2: Selecciona tu carpeta de instalación de Los Sims 4 (o déjalo auto-detectar)";

                OpenInstallerBtn.Content = "🚀 Abrir Instalador Semi-Automático";
            }
        }

        private void GDriveBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Generate and open the quota bypass tutorial
                string tutorialPath = GenerateQuotaBypassTutorial();

                // Open the tutorial HTML
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = tutorialPath,
                    UseShellExecute = true
                });

                // Open Google Drive link
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "https://drive.google.com/file/d/1rOKuoBPT0_PsMpOrxcQzvBEBhqRUxS5x/view?usp=drive_link",
                    UseShellExecute = true
                });

                AnimateStepUnlock(Step1Card);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening link: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TorrentBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Generate and open the BitTorrent tutorial
                string tutorialPath = GenerateBitTorrentTutorial();

                // Open the tutorial HTML
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = tutorialPath,
                    UseShellExecute = true
                });

                // Open BitTorrent download link
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "https://download1076.mediafire.com/rhtwdxromwmgRD_dqfusdCknCbA73qMSxZyQPWp1Fl2AFSXvqq9R4C4HqcAsU0Z9ZaACHaTNUCgWbL1w00qPwmm6wME5tkn7kxVfZ7qWqr9VEFGs4xOE6t4O6QdEuR6kFxaUK19eVCcXLnAUeGAv-MLS8OZkHEM1xXd3hTC_qczw/sd9kovbcpp9zsp8/The+Sims+4+-+All+DLCs.torrent",
                    UseShellExecute = true
                });

                AnimateStepUnlock(Step1Card);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening link: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string GenerateBitTorrentTutorial()
        {
            string tempPath = Path.Combine(Path.GetTempPath(), "bittorrent_tutorial.html");

            string htmlContent = @"<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>BitTorrent Download Tutorial</title>
    <style>
        * {
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }

        body {
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            background: linear-gradient(135deg, #0F172A 0%, #1E293B 100%);
            color: #E5E7EB;
            min-height: 100vh;
            padding: 20px;
        }

        .language-selector {
            position: fixed;
            top: 20px;
            right: 20px;
            display: flex;
            gap: 10px;
            z-index: 1000;
        }

        .lang-btn {
            background: rgba(255, 255, 255, 0.1);
            border: 2px solid #3EC7E8;
            color: white;
            padding: 10px 20px;
            border-radius: 10px;
            cursor: pointer;
            font-weight: 600;
            transition: all 0.3s ease;
        }

        .lang-btn:hover {
            background: #3EC7E8;
            transform: translateY(-2px);
        }

        .lang-btn.active {
            background: #0A6BF2;
            border-color: #0A6BF2;
        }

        .container {
            max-width: 900px;
            margin: 60px auto 0;
            background: rgba(255, 255, 255, 0.05);
            border-radius: 20px;
            padding: 40px;
            box-shadow: 0 20px 60px rgba(0, 0, 0, 0.3);
            border: 2px solid rgba(62, 199, 232, 0.3);
        }

        .header {
            text-align: center;
            margin-bottom: 40px;
        }

        .header h1 {
            font-size: 2.5em;
            color: #3EC7E8;
            margin-bottom: 10px;
            display: flex;
            align-items: center;
            justify-content: center;
            gap: 15px;
        }

        .header p {
            color: #94A3B8;
            font-size: 1.1em;
        }

        .content {
            display: none;
        }

        .content.active {
            display: block;
            animation: fadeIn 0.5s ease;
        }

        @keyframes fadeIn {
            from { opacity: 0; transform: translateY(20px); }
            to { opacity: 1; transform: translateY(0); }
        }

        .intro {
            background: rgba(139, 92, 246, 0.1);
            border-left: 4px solid #8B5CF6;
            padding: 20px;
            border-radius: 10px;
            margin-bottom: 30px;
        }

        .intro h2 {
            color: #8B5CF6;
            margin-bottom: 10px;
        }

        .step {
            background: rgba(255, 255, 255, 0.05);
            border-radius: 15px;
            padding: 25px;
            margin-bottom: 20px;
            border: 2px solid rgba(62, 199, 232, 0.2);
            transition: all 0.3s ease;
        }

        .step:hover {
            border-color: #3EC7E8;
            transform: translateX(5px);
        }

        .step-number {
            display: inline-block;
            background: linear-gradient(135deg, #8B5CF6, #6366F1);
            color: white;
            width: 40px;
            height: 40px;
            border-radius: 50%;
            text-align: center;
            line-height: 40px;
            font-weight: bold;
            font-size: 1.2em;
            margin-right: 15px;
            box-shadow: 0 4px 15px rgba(139, 92, 246, 0.4);
        }

        .step h3 {
            color: #3EC7E8;
            margin-bottom: 15px;
            font-size: 1.3em;
        }

        .step p {
            line-height: 1.8;
            color: #CBD5E1;
            margin-bottom: 10px;
        }

        .step code {
            background: rgba(0, 0, 0, 0.3);
            padding: 3px 8px;
            border-radius: 5px;
            color: #3EC7E8;
            font-family: 'Courier New', monospace;
        }

        .download-btn {
            display: inline-block;
            background: linear-gradient(135deg, #8B5CF6, #6366F1);
            color: white;
            padding: 12px 30px;
            border-radius: 10px;
            text-decoration: none;
            font-weight: bold;
            margin-top: 10px;
            transition: all 0.3s ease;
            box-shadow: 0 4px 15px rgba(139, 92, 246, 0.4);
        }

        .download-btn:hover {
            transform: translateY(-2px);
            box-shadow: 0 6px 20px rgba(139, 92, 246, 0.6);
        }

        .result {
            background: rgba(34, 197, 94, 0.1);
            border-left: 4px solid #22C55E;
            padding: 20px;
            border-radius: 10px;
            text-align: center;
        }

        .result h2 {
            color: #22C55E;
            margin-bottom: 10px;
            font-size: 1.8em;
        }

        .note {
            background: rgba(251, 191, 36, 0.1);
            border-left: 4px solid #F59E0B;
            padding: 15px;
            border-radius: 8px;
            margin-top: 15px;
        }

        .note strong {
            color: #F59E0B;
        }

        ul {
            margin-left: 20px;
            margin-top: 10px;
        }

        li {
            margin-bottom: 8px;
            color: #CBD5E1;
        }

        .emoji {
            font-size: 1.5em;
            margin-right: 10px;
        }
    </style>
</head>
<body>
    <div class=""language-selector"">
        <button class=""lang-btn active"" onclick=""setLanguage('en')"">🇬🇧 English</button>
        <button class=""lang-btn"" onclick=""setLanguage('es')"">🇪🇸 Español</button>
    </div>

    <div class=""container"">
        <!-- ENGLISH VERSION -->
        <div id=""content-en"" class=""content active"">
            <div class=""header"">
                <h1>⚡ BitTorrent Download Guide</h1>
                <p>Simple step-by-step tutorial for downloading DLCs via BitTorrent</p>
            </div>

            <div class=""intro"">
                <h2>📖 What is BitTorrent?</h2>
                <p>BitTorrent is a fast and efficient way to download large files. Instead of downloading from a single server, you download pieces from multiple users, making it much faster for big files like game DLCs.</p>
            </div>

            <div class=""step"">
                <h3><span class=""step-number"">1</span> Download BitTorrent Client</h3>
                <p>First, you need to install a BitTorrent client. We recommend <strong>qBittorrent</strong> (free and no ads).</p>
                <a href=""https://www.qbittorrent.org/download.php"" target=""_blank"" class=""download-btn"">
                    📥 Download qBittorrent
                </a>
                <div class=""note"">
                    <strong>💡 Note:</strong> Install it like any other program. Just click ""Next"" through the installer.
                </div>
            </div>

            <div class=""step"">
                <h3><span class=""step-number"">2</span> Download the .torrent File</h3>
                <p>Click the download link provided by Leuan's Toolkit to get the <code>.torrent</code> file.</p>
                <p>This small file contains information about what you're downloading.</p>
                <div class=""note"">
                    <strong>💡 Tip:</strong> The .torrent file is tiny (a few KB). The actual DLC files will download in the next step.
                </div>
            </div>

            <div class=""step"">
                <h3><span class=""step-number"">3</span> Open the .torrent File</h3>
                <p>Once qBittorrent is installed and the .torrent file is downloaded:</p>
                <ul>
                    <li><span class=""emoji"">📂</span> Find the <code>.torrent</code> file in your Downloads folder</li>
                    <li><span class=""emoji"">🖱️</span> Double-click it, OR drag it into the qBittorrent window</li>
                </ul>
            </div>

            <div class=""step"">
                <h3><span class=""step-number"">4</span> Choose What to Download</h3>
                <p>A window will pop up showing:</p>
                <ul>
                    <li><strong>Files to download:</strong> You can select/deselect specific DLCs (or download all)</li>
                    <li><strong>Save location:</strong> Choose where to save the files (Desktop is recommended)</li>
                </ul>
                <p>Once you're ready, click <code>OK</code> to start downloading.</p>
                <div class=""note"">
                    <strong>💡 Recommendation:</strong> Save to a new folder on your Desktop called ""Sims4_DLCs"" for easy access.
                </div>
            </div>

            <div class=""step"">
                <h3><span class=""step-number"">5</span> Wait for Download to Complete</h3>
                <p>qBittorrent will now download the files. You'll see:</p>
                <ul>
                    <li>Download speed</li>
                    <li>Progress percentage</li>
                    <li>Estimated time remaining</li>
                </ul>
                <p>Once it says <strong>""Seeding""</strong> or <strong>100%</strong>, the download is complete!</p>
            </div>

            <div class=""step"">
                <h3><span class=""step-number"">6</span> Follow Toolkit Instructions</h3>
                <p>Now that your DLCs are downloaded:</p>
                <ul>
                    <li>Go back to <strong>Leuan's Toolkit</strong></li>
                    <li>Follow the <strong>Manual Instalations</strong> steps</li>
                    <li>After doing this you are succesfully in Step 3, you can follow the next steps.</li>
                </ul>
            </div>

            <div class=""result"">
                <h2>✅ You're All Set!</h2>
                <p>🎉 Your DLCs are ready to install. Continue with the toolkit!</p>
            </div>
        </div>

        <!-- SPANISH VERSION -->
        <div id=""content-es"" class=""content"">
            <div class=""header"">
                <h1>⚡ Guía de Descarga con BitTorrent</h1>
                <p>Tutorial paso a paso simple para descargar DLCs vía BitTorrent</p>
            </div>

            <div class=""intro"">
                <h2>📖 ¿Qué es BitTorrent?</h2>
                <p>BitTorrent es una forma rápida y eficiente de descargar archivos grandes. En lugar de descargar desde un solo servidor, descargas piezas de múltiples usuarios, haciéndolo mucho más rápido para archivos grandes como DLCs de juegos.</p>
            </div>

            <div class=""step"">
                <h3><span class=""step-number"">1</span> Descargar Cliente BitTorrent</h3>
                <p>Primero, necesitas instalar un cliente BitTorrent. Recomendamos <strong>qBittorrent</strong> (gratis y sin anuncios).</p>
                <a href=""https://www.qbittorrent.org/download.php"" target=""_blank"" class=""download-btn"">
                    📥 Descargar qBittorrent
                </a>
                <div class=""note"">
                    <strong>💡 Nota:</strong> Instálalo como cualquier otro programa. Solo haz clic en ""Siguiente"" durante la instalación.
                </div>
            </div>

            <div class=""step"">
                <h3><span class=""step-number"">2</span> Descargar el Archivo .torrent</h3>
                <p>Haz clic en el enlace de descarga proporcionado por Leuan's Toolkit para obtener el archivo <code>.torrent</code>.</p>
                <p>Este pequeño archivo contiene información sobre lo que vas a descargar.</p>
                <div class=""note"">
                    <strong>💡 Consejo:</strong> El archivo .torrent es pequeño (unos pocos KB). Los archivos DLC reales se descargarán en el siguiente paso.
                </div>
            </div>

            <div class=""step"">
                <h3><span class=""step-number"">3</span> Abrir el Archivo .torrent</h3>
                <p>Una vez que qBittorrent esté instalado y el archivo .torrent descargado:</p>
                <ul>
                    <li><span class=""emoji"">📂</span> Encuentra el archivo <code>.torrent</code> en tu carpeta de Descargas</li>
                    <li><span class=""emoji"">🖱️</span> Haz doble clic en él, O arrástralo a la ventana de qBittorrent</li>
                </ul>
            </div>

            <div class=""step"">
                <h3><span class=""step-number"">4</span> Elegir Qué Descargar</h3>
                <p>Aparecerá una ventana mostrando:</p>
                <ul>
                    <li><strong>Archivos a descargar:</strong> Puedes seleccionar/deseleccionar DLCs específicos (o descargar todos)</li>
                    <li><strong>Ubicación de guardado:</strong> Elige dónde guardar los archivos (se recomienda el Escritorio)</li>
                </ul>
                <p>Una vez listo, haz clic en <code>OK</code> para comenzar la descarga.</p>
                <div class=""note"">
                    <strong>💡 Recomendación:</strong> Guarda en una carpeta nueva en tu Escritorio llamada ""Sims4_DLCs"" para fácil acceso.
                </div>
            </div>

            <div class=""step"">
                <h3><span class=""step-number"">5</span> Esperar a que Complete la Descarga</h3>
                <p>qBittorrent ahora descargará los archivos. Verás:</p>
                <ul>
                    <li>Velocidad de descarga</li>
                    <li>Porcentaje de progreso</li>
                    <li>Tiempo estimado restante</li>
                </ul>
                <p>¡Una vez que diga <strong>""Sembrando""</strong> o <strong>100%</strong>, la descarga está completa!</p>
            </div>

            <div class=""step"">
                <h3><span class=""step-number"">6</span> Seguir las Instrucciones del Toolkit</h3>
                <p>Ahora que tus DLCs están descargados:</p>
                <ul>
                    <li>Regresa a <strong>Leuan's Toolkit</strong></li>
                    <li>Sigue los pasos de la <strong>Instalación Manual</strong></li>
                    <li>Luego de hacer todo, estaras en el Paso 3, puedes seguir los siguientes desde aqui.</li>
                </ul>
            </div>

            <div class=""result"">
                <h2>✅ ¡Todo Listo!</h2>
                <p>🎉 Tus DLCs están listos para instalar. ¡Continúa con el toolkit!</p>
            </div>
        </div>
    </div>

    <script>
        function setLanguage(lang) {
            document.querySelectorAll('.content').forEach(el => {
                el.classList.remove('active');
            });
            document.getElementById('content-' + lang).classList.add('active');
            document.querySelectorAll('.lang-btn').forEach(btn => {
                btn.classList.remove('active');
            });
            event.target.classList.add('active');
        }
    </script>
</body>
</html>";

            File.WriteAllText(tempPath, htmlContent);
            return tempPath;
        }

        private string GenerateQuotaBypassTutorial()
        {
            string tempPath = Path.Combine(Path.GetTempPath(), "gdrive_quota_bypass.html");

            string htmlContent = @"<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Google Drive Quota Bypass Tutorial</title>
    <style>
        * {
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }

        body {
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            background: linear-gradient(135deg, #0F172A 0%, #1E293B 100%);
            color: #E5E7EB;
            min-height: 100vh;
            padding: 20px;
        }

        .language-selector {
            position: fixed;
            top: 20px;
            right: 20px;
            display: flex;
            gap: 10px;
            z-index: 1000;
        }

        .lang-btn {
            background: rgba(255, 255, 255, 0.1);
            border: 2px solid #3EC7E8;
            color: white;
            padding: 10px 20px;
            border-radius: 10px;
            cursor: pointer;
            font-weight: 600;
            transition: all 0.3s ease;
        }

        .lang-btn:hover {
            background: #3EC7E8;
            transform: translateY(-2px);
        }

        .lang-btn.active {
            background: #0A6BF2;
            border-color: #0A6BF2;
        }

        .container {
            max-width: 900px;
            margin: 60px auto 0;
            background: rgba(255, 255, 255, 0.05);
            border-radius: 20px;
            padding: 40px;
            box-shadow: 0 20px 60px rgba(0, 0, 0, 0.3);
            border: 2px solid rgba(62, 199, 232, 0.3);
        }

        .header {
            text-align: center;
            margin-bottom: 40px;
        }

        .header h1 {
            font-size: 2.5em;
            color: #3EC7E8;
            margin-bottom: 10px;
            display: flex;
            align-items: center;
            justify-content: center;
            gap: 15px;
        }

        .header p {
            color: #94A3B8;
            font-size: 1.1em;
        }

        .content {
            display: none;
        }

        .content.active {
            display: block;
            animation: fadeIn 0.5s ease;
        }

        @keyframes fadeIn {
            from { opacity: 0; transform: translateY(20px); }
            to { opacity: 1; transform: translateY(0); }
        }

        .intro {
            background: rgba(239, 68, 68, 0.1);
            border-left: 4px solid #EF4444;
            padding: 20px;
            border-radius: 10px;
            margin-bottom: 30px;
        }

        .intro h2 {
            color: #EF4444;
            margin-bottom: 10px;
        }

        .step {
            background: rgba(255, 255, 255, 0.05);
            border-radius: 15px;
            padding: 25px;
            margin-bottom: 20px;
            border: 2px solid rgba(62, 199, 232, 0.2);
            transition: all 0.3s ease;
        }

        .step:hover {
            border-color: #3EC7E8;
            transform: translateX(5px);
        }

        .step-number {
            display: inline-block;
            background: linear-gradient(135deg, #3EC7E8, #0A6BF2);
            color: white;
            width: 40px;
            height: 40px;
            border-radius: 50%;
            text-align: center;
            line-height: 40px;
            font-weight: bold;
            font-size: 1.2em;
            margin-right: 15px;
            box-shadow: 0 4px 15px rgba(62, 199, 232, 0.4);
        }

        .step h3 {
            color: #3EC7E8;
            margin-bottom: 15px;
            font-size: 1.3em;
        }

        .step p {
            line-height: 1.8;
            color: #CBD5E1;
        }

        .step code {
            background: rgba(0, 0, 0, 0.3);
            padding: 3px 8px;
            border-radius: 5px;
            color: #3EC7E8;
            font-family: 'Courier New', monospace;
        }

        .why-section {
            background: rgba(251, 191, 36, 0.1);
            border-left: 4px solid #F59E0B;
            padding: 20px;
            border-radius: 10px;
            margin: 30px 0;
        }

        .why-section h2 {
            color: #F59E0B;
            margin-bottom: 15px;
        }

        .result {
            background: rgba(34, 197, 94, 0.1);
            border-left: 4px solid #22C55E;
            padding: 20px;
            border-radius: 10px;
            text-align: center;
        }

        .result h2 {
            color: #22C55E;
            margin-bottom: 10px;
            font-size: 1.8em;
        }

        .credit {
            text-align: center;
            margin-top: 30px;
            padding-top: 20px;
            border-top: 1px solid rgba(255, 255, 255, 0.1);
            color: #94A3B8;
            font-size: 0.9em;
        }

        .credit a {
            color: #3EC7E8;
            text-decoration: none;
        }

        .credit a:hover {
            text-decoration: underline;
        }

        ul {
            margin-left: 20px;
            margin-top: 10px;
        }

        li {
            margin-bottom: 8px;
            color: #CBD5E1;
        }
    </style>
</head>
<body>
    <div class=""language-selector"">
        <button class=""lang-btn active"" onclick=""setLanguage('en')"">🇬🇧 English</button>
        <button class=""lang-btn"" onclick=""setLanguage('es')"">🇪🇸 Español</button>
    </div>

    <div class=""container"">
        <!-- ENGLISH VERSION -->
        <div id=""content-en"" class=""content active"">
            <div class=""header"">
                <h1>🚀 Google Drive Quota Bypass</h1>
                <p>How to bypass the annoying ""Download quota exceeded"" error</p>
            </div>

            <div class=""intro"">
                <h2>⚠️ The Problem</h2>
                <p>Google Drive places an annoying download quota on files, blocking downloads whenever ""too many users"" have accessed a file. If you run into the <strong>""Download quota exceeded""</strong> message, here's how to bypass it using your own Google account.</p>
            </div>

            <div class=""step"">
                <h3><span class=""step-number"">1</span> Add the File to Your Drive</h3>
                <p>On the upper-right corner of the blocked file page, click:</p>
                <p><code>Organize → Add shortcut to Drive</code></p>
                <p>Add it anywhere in <strong>My Drive</strong>.</p>
            </div>

            <div class=""step"">
                <h3><span class=""step-number"">2</span> Create a New Folder</h3>
                <p>📁 Make a fresh folder in your Google Drive.</p>
                <p>This helps trick Google's quota system.</p>
            </div>

            <div class=""step"">
                <h3><span class=""step-number"">3</span> Move the Shortcut Into the Folder</h3>
                <p>Drag the shortcut you added into this new folder.</p>
            </div>

            <div class=""step"">
                <h3><span class=""step-number"">4</span> Download the Folder</h3>
                <p>From your own Google Drive, right-click the folder →</p>
                <p><code>➡️ Download</code></p>
                <p>Google Drive will now zip the file, ignoring the quota restriction.</p>
            </div>

            <div class=""why-section"">
                <h2>🧠 Why This Works</h2>
                <p>Google won't let you download the file directly… But it will:</p>
                <ul>
                    <li>Zip the entire folder (including the file)</li>
                    <li>Spend CPU time</li>
                    <li>Use more bandwidth</li>
                    <li>And then let you download it freely</li>
                </ul>
                <p style=""margin-top: 15px;""><strong>🤦 This is unbelievably inefficient and anti-consumer.</strong></p>
            </div>

            <div class=""result"">
                <h2>✅ Result</h2>
                <p>🎉 The blocked file is now downloadable, no quota wall!</p>
            </div>

            <div class=""credit"">
                <p>Source: <a href=""https://www.reddit.com/r/google/comments/nfsgal/google_drive_download_quota_limit_exceeded_bypass/"" target=""_blank"">Reddit Post</a></p>
                <p>All credits to the original Reddit post.</p>
            </div>
        </div>

        <!-- SPANISH VERSION -->
        <div id=""content-es"" class=""content"">
            <div class=""header"">
                <h1>🚀 Bypass de Cuota de Google Drive</h1>
                <p>Cómo evitar el molesto error ""Cuota de descarga excedida""</p>
            </div>

            <div class=""intro"">
                <h2>⚠️ El Problema</h2>
                <p>Google Drive coloca una molesta cuota de descarga en los archivos, bloqueando las descargas cuando ""demasiados usuarios"" han accedido a un archivo. Si te encuentras con el mensaje <strong>""Cuota de descarga excedida""</strong>, aquí te mostramos cómo evitarlo usando tu propia cuenta de Google.</p>
            </div>

            <div class=""step"">
                <h3><span class=""step-number"">1</span> Agregar el Archivo a tu Drive</h3>
                <p>En la esquina superior derecha de la página del archivo bloqueado, haz clic en:</p>
                <p><code>Organizar → Agregar acceso directo a Drive</code></p>
                <p>Agrégalo en cualquier lugar de <strong>Mi unidad</strong>.</p>
            </div>

            <div class=""step"">
                <h3><span class=""step-number"">2</span> Crear una Nueva Carpeta</h3>
                <p>📁 Crea una carpeta nueva en tu Google Drive.</p>
                <p>Esto ayuda a engañar al sistema de cuotas de Google.</p>
            </div>

            <div class=""step"">
                <h3><span class=""step-number"">3</span> Mover el Acceso Directo a la Carpeta</h3>
                <p>Arrastra el acceso directo que agregaste a esta nueva carpeta.</p>
            </div>

            <div class=""step"">
                <h3><span class=""step-number"">4</span> Descargar la Carpeta</h3>
                <p>Desde tu propio Google Drive, haz clic derecho en la carpeta →</p>
                <p><code>➡️ Descargar</code></p>
                <p>Google Drive ahora comprimirá el archivo en ZIP, ignorando la restricción de cuota.</p>
            </div>

            <div class=""why-section"">
                <h2>🧠 Por Qué Funciona</h2>
                <p>Google no te dejará descargar el archivo directamente… Pero sí:</p>
                <ul>
                    <li>Comprimirá toda la carpeta (incluyendo el archivo)</li>
                    <li>Gastará tiempo de CPU</li>
                    <li>Usará más ancho de banda</li>
                    <li>Y luego te dejará descargarlo libremente</li>
                </ul>
                <p style=""margin-top: 15px;""><strong>🤦 Esto es increíblemente ineficiente y anti-consumidor.</strong></p>
            </div>

            <div class=""result"">
                <h2>✅ Resultado</h2>
                <p>🎉 ¡El archivo bloqueado ahora se puede descargar, sin límite de cuota!</p>
            </div>

            <div class=""credit"">
                <p>Fuente: <a href=""https://www.reddit.com/r/google/comments/nfsgal/google_drive_download_quota_limit_exceeded_bypass/"" target=""_blank"">Publicación de Reddit</a></p>
                <p>Todos los créditos a la publicación original de Reddit.</p>
            </div>
        </div>
    </div>

    <script>
        function setLanguage(lang) {
            // Hide all content
            document.querySelectorAll('.content').forEach(el => {
                el.classList.remove('active');
            });

            // Show selected language
            document.getElementById('content-' + lang).classList.add('active');

            // Update button states
            document.querySelectorAll('.lang-btn').forEach(btn => {
                btn.classList.remove('active');
            });
            event.target.classList.add('active');
        }
    </script>
</body>
</html>";

            File.WriteAllText(tempPath, htmlContent);
            return tempPath;
        }

        private void AnimateStepUnlock(Border stepCard)
        {
            var fadeIn = new DoubleAnimation
            {
                To = 1,
                Duration = TimeSpan.FromMilliseconds(600),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };

            stepCard.BeginAnimation(OpacityProperty, fadeIn);

            // If Step1 was unlocked, unlock Step2
            if (stepCard == Step1Card)
            {
                var step2Fade = new DoubleAnimation
                {
                    To = 1,
                    Duration = TimeSpan.FromMilliseconds(600),
                    BeginTime = TimeSpan.FromMilliseconds(300),
                    EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
                };
                Step2Card.BeginAnimation(OpacityProperty, step2Fade);
            }
        }

        private void OpenInstallerBtn_Click(object sender, RoutedEventArgs e)
        {
            var installerWindow = new SemiAutoInstallerWindow
            {
                Owner = Application.Current.MainWindow
            };

            var fadeOut = new DoubleAnimation
            {
                To = 0,
                Duration = TimeSpan.FromMilliseconds(300)
            };

            fadeOut.Completed += (s, args) =>
            {
                this.Close();
                installerWindow.Opacity = 0;
                installerWindow.Show();

                var fadeIn = new DoubleAnimation
                {
                    To = 1,
                    Duration = TimeSpan.FromMilliseconds(300)
                };
                installerWindow.BeginAnimation(Window.OpacityProperty, fadeIn);
            };

            this.BeginAnimation(Window.OpacityProperty, fadeOut);
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            var welcomeWindow = new leuFastWelcomeWindow();

            var fadeOut = new DoubleAnimation
            {
                To = 0,
                Duration = TimeSpan.FromMilliseconds(300)
            };

            fadeOut.Completed += (s, args) =>
            {
                this.Close();
                welcomeWindow.Opacity = 0;
                welcomeWindow.Show();

                var fadeIn = new DoubleAnimation
                {
                    To = 1,
                    Duration = TimeSpan.FromMilliseconds(300)
                };
                welcomeWindow.BeginAnimation(Window.OpacityProperty, fadeIn);
            };

            this.BeginAnimation(Window.OpacityProperty, fadeOut);
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            var fadeOut = new DoubleAnimation
            {
                To = 0,
                Duration = TimeSpan.FromMilliseconds(300)
            };

            fadeOut.Completed += (s, args) => this.Close();
            this.BeginAnimation(Window.OpacityProperty, fadeOut);
        }
    }
}