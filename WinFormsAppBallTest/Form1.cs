using System;

namespace WinFormsAppBallTest
{
    public partial class Form1 : Form
    {
        private Thread drawingThread;
        private bool keepDrawing;

        private Rectangle lastCircle;
        private enum Directions
        {
            Right = 1,
            Left = 2,
            Down = 3,
            Up = 4
        }
        private int CurrentDirection = 1;
        private int xCordinate = -1;
        private int yCordinate = 0;
        private bool ballAnimationForward = true;
        private int CircleWidth = 90;

        public Form1()
        {
            InitializeComponent();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            // If there's already a drawing thread, stop it
            if (drawingThread != null && drawingThread.IsAlive)
            {
                keepDrawing = false;
                drawingThread.Join(); // Wait for the thread to finish
            }

            // Create and start a new thread
            keepDrawing = true;
            drawingThread = new Thread(new ThreadStart(DrawCircles));
            drawingThread.Start();
        }

        private int returnDirection()
        {

            if (xCordinate == 0 && yCordinate == 0)
                ballAnimationForward = true;

            if (xCordinate == 0)
            {
                if (yCordinate % (CircleWidth * 2) == 0)
                {
                    if (ballAnimationForward) return (int)Directions.Right;
                    else return (int)Directions.Up;
                }

                if ((yCordinate - CircleWidth) % (CircleWidth * 2) == 0)
                {
                    if (ballAnimationForward) return (int)Directions.Down;
                    else return (int)Directions.Right;
                }

                if (yCordinate == ClientSize.Height - CircleWidth)
                {
                    if (xCordinate == 0 && ClientSize.Height % (CircleWidth * 2) >= CircleWidth)
                    {
                        ballAnimationForward = false;
                        return (int)Directions.Right;
                    }

                    if (ballAnimationForward)
                        return (int)Directions.Right;
                    else
                        return (int)Directions.Up;
                }
            }

            if (xCordinate == ClientSize.Width - CircleWidth)
            {
                if (yCordinate % (CircleWidth * 2) == 0)
                {
                    if (ballAnimationForward) return (int)Directions.Down;
                    else return (int)Directions.Left;
                }
                if ((yCordinate - CircleWidth) % (CircleWidth * 2) == 0)
                {
                    if (ballAnimationForward) return (int)Directions.Left;
                    else return (int)Directions.Up;
                }

                if (yCordinate == ClientSize.Height - CircleWidth)
                {
                    if (xCordinate == ClientSize.Width - CircleWidth && ClientSize.Height % (CircleWidth * 2) <= CircleWidth)
                    {
                        ballAnimationForward = false;
                        return (int)Directions.Left;
                    }

                    if (ballAnimationForward)
                        return (int)Directions.Left;
                    else
                        return (int)Directions.Up;
                }
            }
            return CurrentDirection;
        }

        private void DrawCircles()
        {
            while (keepDrawing)
            {
                CurrentDirection = returnDirection();

                if ((Directions)CurrentDirection == Directions.Right) xCordinate++;
                if ((Directions)CurrentDirection == Directions.Left) xCordinate--;
                if ((Directions)CurrentDirection == Directions.Up) yCordinate--;
                if ((Directions)CurrentDirection == Directions.Down) yCordinate++;

                Rectangle newCircle = new Rectangle(xCordinate, yCordinate, CircleWidth, CircleWidth);

                // Use Invoke to safely update the UI from the thread
                this.Invoke(new Action(() => {
                    // Clear the previous circle by invalidating the form (this erases the previous drawing)
                    Invalidate();

                    // Draw the new circle
                    using (Graphics g = this.CreateGraphics())
                    {
                        Pen pen = new Pen(Color.Red, 2);
                        g.DrawEllipse(pen, newCircle);
                    }

                    // Update the last circle location
                    lastCircle = newCircle;
                }));
                Thread.Sleep(0); // Adjust delay as needed
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            // If there's a circle to draw, redraw it when the form repaints
            if (lastCircle != Rectangle.Empty)
            {
                using (Pen pen = new Pen(Color.Red, 2))
                {
                    e.Graphics.DrawEllipse(pen, lastCircle);
                }
            }

            //base.OnPaint(e);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Stop the drawing thread when the form is closing
            keepDrawing = false;
            if (drawingThread != null && drawingThread.IsAlive)
            {
                drawingThread.Join(); // Wait for the thread to finish
            }
            base.OnFormClosing(e);
        }

    }
}