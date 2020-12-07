using System.Drawing;

namespace Fracta
{
    /// <summary>
    /// Класс, который хранит всё что может понадобиться для рисования чего-либо.
    /// </summary>
    public class DrawingContext
    {
        /// <summary>
        /// Создаёт DrawingContext из картинки.
        /// </summary>
        public DrawingContext(Image image)
        {
            Graphics = Graphics.FromImage(image);
            Image = image;
        }

        /// <summary>
        /// Graphics — невероятно важная вещь.
        /// Лучше прочитать документацию по этому объекту, чтобы понять, что без него ничего не нарисуешь.
        /// </summary>
        public Graphics Graphics { get; }

        /// <summary>
        /// Картинка, на котороый происходит рисование.
        /// </summary>
        public Image Image { get; }
    }
}