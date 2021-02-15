using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


namespace ArmourGan
{
    // TODO dude.
#nullable disable
    public class TextureCache
    {
        public static Texture2D pixel;
        public static void LoadTextures(ContentManager content)
        {
            pixel = new Texture2D(Main.graphics.GraphicsDevice, 1, 1);
            pixel.SetData(new Color[] { Color.White });

            //FrontBicep = content.Load<Texture2D>("BodyParts/FrontBicep");
            //FrontForearm = content.Load<Texture2D>("BodyParts/FrontForearm");
            //BackBicep = content.Load<Texture2D>("BodyParts/BackBicep");
            //BackForearm = content.Load<Texture2D>("BodyParts/BackForearm");
        }
    }
}
