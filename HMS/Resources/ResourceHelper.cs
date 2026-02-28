using System;
using System.Drawing;
using System.IO;

namespace HMS.Resources
{
    // Helper to load optional resources (logo.png) from the application folder or working directory.
    public static class ResourceHelper
    {
        public static Image LoadLogo()
        {
            try
            {
                string baseDir = AppDomain.CurrentDomain.BaseDirectory ?? Directory.GetCurrentDirectory();
                string[] candidates = new[]
                {
                    Path.Combine(baseDir, "logo.png"),
                    Path.Combine(Directory.GetCurrentDirectory(), "logo.png"),
                    Path.Combine(baseDir, "Resources", "logo.png")
                };

                // Also support environment variable override
                var env = Environment.GetEnvironmentVariable("HMS_LOGO_PATH");
                if (!string.IsNullOrWhiteSpace(env))
                {
                    candidates = new[] { env }.Concat(candidates).ToArray();
                }

                // Walk up a few parent directories to detect a project-level Resources folder
                var dir = new DirectoryInfo(baseDir);
                for (int i = 0; i < 6 && dir != null; i++)
                {
                    var candidate1 = Path.Combine(dir.FullName, "HMS", "Resources", "logo.png");
                    var candidate2 = Path.Combine(dir.FullName, "Resources", "logo.png");
                    candidates = candidates.Concat(new[] { candidate1, candidate2 }).ToArray();
                    dir = dir.Parent;
                }

                foreach (var path in candidates)
                {
                    try
                    {
                        if (string.IsNullOrWhiteSpace(path)) continue;
                        if (File.Exists(path))
                        {
                            // Image.FromFile keeps the file locked while Image is in use. It's acceptable for a simple app.
                            return Image.FromFile(path);
                        }
                    }
                    catch
                    {
                        // ignore and continue
                    }
                }
            }
            catch
            {
                // Ignore errors and return null if image cannot be loaded.
            }

            return null;
        }
    }
}
