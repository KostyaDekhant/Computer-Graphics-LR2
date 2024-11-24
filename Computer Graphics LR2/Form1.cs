using static Computer_Graphics_LR2.Form1;

namespace Computer_Graphics_LR2
{
    public partial class Form1 : Form
    {
        private Renderer renderer;
        private List<Polygon> polygons;
        public static int time = 0;
        public Form1()
        {
            createPoly();
            this.ClientSize = new Size(1024, 512);
            this.DoubleBuffered = true;
            this.Load += Form1_Load;
            this.Paint += Form1_Paint;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            renderer = new Renderer(512, 512);
            this.Invalidate(); //Перерисовать форму
        }

        private void createPoly()
        {
            polygons = new List<Polygon>
            {
                new Polygon(new Point3D[]
                {
                    new Point3D(200, 130, 200),
                    new Point3D(200, 380, 200),
                    new Point3D(320, 380, 200),
                    new Point3D(320, 130, 200),
                }, Color.Red),

                new Polygon(new Point3D[]
                {
                    new Point3D(250, 250, 250),
                    new Point3D(350, 150, 50),
                    new Point3D(400, 350, 50),
                }, Color.Green),

                new Polygon(new Point3D[]
                {
                    new Point3D(150, 220, 100),
                    new Point3D(150, 300, 100),
                    new Point3D(370, 300, 100),
                    new Point3D(370, 220, 100),
                }, Color.Blue),

                new Polygon(new Point3D[]
                {
                    new Point3D(150, 250, 50),
                    new Point3D(180, 150, 150),
                    new Point3D(150, 350, 50),
                }, Color.Yellow),
            };
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Rectangle frameRect = new Rectangle(0, 0, 512, 512);
            Rectangle zRect = new Rectangle(512, 0, 512, 512);
            renderer.Render(polygons, e.Graphics, frameRect, zRect);
        }
    }
}