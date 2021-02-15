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
        public static Texture2D d;
        public static Texture2D d1;
        public static void LoadTextures(ContentManager content)
        {
            pixel = new Texture2D(Main.graphics.GraphicsDevice, 1, 1);
            pixel.SetData(new Color[] { Color.White });

            d = content.Load<Texture2D>("MachineLearning/DataSet/DalantiniumGreathelm");
            d1 = content.Load<Texture2D>("MachineLearning/DataSet/DalantiniumGreathelm_Head");
        }
    }
}
