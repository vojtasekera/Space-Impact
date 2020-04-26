using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Space_Impact
{
    public class Texture
    {
        static Dictionary<string, Texture> Data = new Dictionary<string, Texture>();

        public int offx, offy;
        public List<Bitmap> images;

        public Texture(int x, int y, params string[] file_names)
        {
            offx = x;
            offy = y;
            images = new List<Bitmap>();
            foreach (string name in file_names)
            {
                images.Add((Bitmap)Bitmap.FromFile(name));
            }
        }

        static public Texture FromKey(string key)
        {
            Texture a;

            if ( !Data.TryGetValue(key, out a)) Data.TryGetValue("null", out a);
            return a;
        }

        public static void Load(string path)
        {
            List<string> fields;
            string name = "";
            int xoff, yoff;

            string[] lines = System.IO.File.ReadAllLines(path);

            foreach (string line in lines)
            {
                try
                {
                    fields = line.Split(';').ToList();
                    name = fields[0];
                    xoff = int.Parse(fields[1]);
                    yoff = int.Parse(fields[2]);
                    fields.RemoveRange(0, 3);
                    Data.Add(name, new Texture(xoff, yoff, fields.ToArray()));
                }
                catch (Exception)
                {
                    MessageBox.Show("Chyba při načítání obrázků");
                }
            }
        }

        public Bitmap Image()
        {
            return this.images.First();
        }
    }

    class Visuals
    {
        bool loop = true;
        int delay = Constants.graphics_flash_interval, tick;
        int image;
        Texture data;

        public void Draw(MovingObject parent, Graphics sender)
        {
            if (data != null)
                sender.DrawImage(data.images[image], parent.x + data.offx, parent.y + data.offy);
            if (loop)
            {
                tick++;
                tick %= delay;

                if (tick == 0) Next();
            }
        }

        public void Next()
        {
            image++;
            image %= data.images.Count();
        }

        public void Stop()
        {
            loop = false;
        }

        public Visuals(Texture t)
        {
            data = t;
        }
    }

}
