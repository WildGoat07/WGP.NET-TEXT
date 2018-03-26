using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;

namespace WGP.TEXT
{
    /// <summary>
    /// Like the SFML.Graphics.Text, it draws text. It is used as an alternate way of generating text because the SFML.System.Text is bugged.
    /// </summary>
    public class Text : Transformable, Drawable
    {
        /// <summary>
        /// String to display.
        /// </summary>
        public string String
        {
            get => _string;
            set
            {
                _string = value;
                requireUpdate = true;
            }
        }
        /// <summary>
        /// The used font.
        /// </summary>
        public Font Font
        {
            get => _font;
            set
            {
                _font = value;
                requireUpdate = true;
            }
        }
        /// <summary>
        /// The character size.
        /// </summary>
        public uint CharSize => Font.CharSize;
        /// <summary>
        /// The color of the text.
        /// </summary>
        public Color Color
        {
            get => _color;
            set
            {
                _color = value;
                requireUpdate = true;
            }
        }
        /// <summary>
        /// The style of the text.
        /// </summary>
        public SFML.Graphics.Text.Styles Style
        {
            get => _style;
            set
            {
                _style = value;
                requireUpdate = true;
            }
        }
        private string _string;
        private List<Glyph> glyphs;
        private bool requireUpdate;
        private Font _font;
        private Color _color;
        private RectangleShape underline;
        private RectangleShape strikeThrough;
        private SFML.Graphics.Text.Styles _style;
        private Sprite buffer;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="text">Text to display.</param>
        /// <param name="font">The used font.</param>
        /// <param name="color">The color of the text.</param>
        /// <param name="styles">The style of the text.</param>
        public Text(string text = "", Font font = default, Color color = default, SFML.Graphics.Text.Styles styles = default)
        {
            glyphs = new List<Glyph>();
            String = text;
            Font = font;
            Color = color;
            Style = styles;
            buffer = new Sprite();
            underline = new RectangleShape();
            strikeThrough = new RectangleShape();
        }
        /// <summary>
        /// Updates the internal components. Shouldn't be used normaly.
        /// </summary>
        public void Update()
        {
            if (requireUpdate && Font != null)
            {
                glyphs.Clear();
                foreach (var item in String)
                {
                    glyphs.Add(Font.GetGlyph(item));
                }
                requireUpdate = false;
                buffer.Texture = Font.Texture;
                buffer.Color = Color;
                if ((Style & SFML.Graphics.Text.Styles.Underlined) != 0)
                {
                    underline.FillColor = Color;
                    underline.Position = new SFML.System.Vector2f(0, Font.UnderlinePosition);
                }
                if ((Style & SFML.Graphics.Text.Styles.StrikeThrough) != 0)
                {
                    strikeThrough.FillColor = Color;
                    strikeThrough.Size = new SFML.System.Vector2f(0, Font.UnderlineThickness);
                    strikeThrough.Position = new SFML.System.Vector2f(0, (float)CharSize / -3 + strikeThrough.Size.Y / 2);
                }
            }
        }
        public void Draw(RenderTarget target, RenderStates states)
        {
            states.Transform *= Transform;
            if ((Style & SFML.Graphics.Text.Styles.Italic) != 0)
            {
                Transform tr = new Transform(1, -.2f, 0, 0, 1, 0, 0, 0, 1);
                states.Transform *= tr;
            }
            Update();
            var secStates = new RenderStates(states);
            for (int i = 0;i<glyphs.Count;i++)
            {
                if (String[i] != '\n')
                {
                    buffer.TextureRect = glyphs[i].TextureRect;
                    buffer.Position = new SFML.System.Vector2f(glyphs[i].Bounds.Left, glyphs[i].Bounds.Top);
                    target.Draw(buffer, secStates);
                    if ((Style & SFML.Graphics.Text.Styles.Underlined) != 0)
                    {
                        underline.Size = new SFML.System.Vector2f(glyphs[i].GetGlyphAdvancePatch(), Font.UnderlineThickness);
                        target.Draw(underline, secStates);
                    }
                    if ((Style & SFML.Graphics.Text.Styles.StrikeThrough) != 0)
                    {
                        strikeThrough.Size = new SFML.System.Vector2f(glyphs[i].GetGlyphAdvancePatch(), Font.UnderlineThickness);
                        target.Draw(strikeThrough, secStates);
                    }

                    secStates.Transform.Translate(glyphs[i].GetGlyphAdvancePatch(), 0);
                }
                else
                {
                    secStates = new RenderStates(states);
                    secStates.Transform.Translate(0, Font.LineSpacing);
                }
            }
        }
        /// <summary>
        /// Returns the local bounds of the text.
        /// </summary>
        /// <returns>Local bounds.</returns>
        public FloatRect GetLocalBounds()
        {
            Update();
            SFML.System.Vector2f topleft = new SFML.System.Vector2f(9852, 9852), botright = new SFML.System.Vector2f();
            SFML.System.Vector2f offset = new SFML.System.Vector2f();
            for(int i = 0;i<String.Count();i++)
            {
                if (String[i] == '\n')
                {
                    offset.Y += Font.LineSpacing;
                    offset.X = 0;
                }
                else
                {
                    topleft.X = Utilities.Min(offset.X + glyphs[i].Bounds.Left, topleft.X);
                    topleft.Y = Utilities.Min(offset.Y + glyphs[i].Bounds.Top, topleft.Y);

                    botright.X = Utilities.Max(offset.X + glyphs[i].Bounds.Width + glyphs[i].Bounds.Left, botright.X);
                    botright.Y = Utilities.Max(offset.Y + glyphs[i].Bounds.Height + glyphs[i].Bounds.Top, botright.Y);

                    offset.X += glyphs[i].GetGlyphAdvancePatch();
                }
            }
            return new FloatRect(topleft, botright - topleft);
        }
        /// <summary>
        /// Returns the global bounds of the text.
        /// </summary>
        /// <returns>The global bounds.</returns>
        public FloatRect GetGlobalBounds()
        {
            return Transform.TransformRect(GetLocalBounds());
        }
        /// <summary>
        /// Returns a specified character position. The position is in its local bounds.
        /// </summary>
        /// <param name="pos">Character to find.</param>
        /// <returns>Character position.</returns>
        public SFML.System.Vector2f FindCharacterPos(uint pos)
        {
            Update();
            SFML.System.Vector2f offset = new SFML.System.Vector2f();
            for(int i = 0;i<pos;i++)
            {
                offset.X += glyphs[i].GetGlyphAdvancePatch();
                if (String[i] == '\n')
                {
                    offset.X = 0;
                    offset.Y += Font.LineSpacing;
                }
            }
            return offset;
        }
    }
    internal static class GlyphPatcher
    {
        internal static float GetGlyphAdvancePatch(this Glyph glyph)
        {
            var bytes = BitConverter.GetBytes(glyph.Advance);
            return BitConverter.ToSingle(bytes, 0);
        }
    }
}
