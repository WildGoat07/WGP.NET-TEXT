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
            get => CornersColor[0];
            set
            {
                for (int i = 0; i < 4; i++)
                    CornersColor[i] = value;
            }
        }
        /// <summary>
        /// The color of the 4 corners of each glyph.
        /// </summary>
        public Color[] CornersColor
        {
            get
            {
                return _color;
            }
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
        private Color[] _color;
        private RectangleShape underline;
        private RectangleShape strikeThrough;
        private SFML.Graphics.Text.Styles _style;
        private Vertex[] buffer;

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
            Style = styles;
            underline = new RectangleShape();
            strikeThrough = new RectangleShape();
            CornersColor = new Color[4];
            Color = color;
            requireUpdate = true;
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
                    glyphs.Add(Font.GetGlyph(item, (Style & SFML.Graphics.Text.Styles.Bold) != 0));
                }
                buffer = new Vertex[String.Count() * 4];
                SFML.System.Vector2f offset = new SFML.System.Vector2f();
                for(int i = 0;i < glyphs.Count();i++)
                {
                    if (String[i] != '\n')
                    {
                        var tmp = glyphs[i];
                        for (int j = 0; j < 4; j++)
                            buffer[i * 4 + j].Color = CornersColor[j];
                        buffer[i * 4].TexCoords = new SFML.System.Vector2f(tmp.TextureRect.Left, tmp.TextureRect.Top);
                        buffer[i * 4 + 1].TexCoords = new SFML.System.Vector2f(tmp.TextureRect.Left + tmp.TextureRect.Width, tmp.TextureRect.Top);
                        buffer[i * 4 + 2].TexCoords = new SFML.System.Vector2f(tmp.TextureRect.Left + tmp.TextureRect.Width, tmp.TextureRect.Top + tmp.TextureRect.Height);
                        buffer[i * 4 + 3].TexCoords = new SFML.System.Vector2f(tmp.TextureRect.Left, tmp.TextureRect.Top + tmp.TextureRect.Height);

                        buffer[i * 4].Position = offset + new SFML.System.Vector2f(tmp.Bounds.Left, tmp.Bounds.Top);
                        buffer[i * 4 + 1].Position = offset + new SFML.System.Vector2f(tmp.Bounds.Left + tmp.Bounds.Width, tmp.Bounds.Top);
                        buffer[i * 4 + 2].Position = offset + new SFML.System.Vector2f(tmp.Bounds.Left + tmp.Bounds.Width, tmp.Bounds.Top + tmp.Bounds.Height);
                        buffer[i * 4 + 3].Position = offset + new SFML.System.Vector2f(tmp.Bounds.Left, tmp.Bounds.Top + tmp.Bounds.Height);
                        offset.X += tmp.Advance;
                    }
                    else
                    {
                        offset.X = 0;
                        offset.Y += Font.LineSpacing;
                    }
                }
                requireUpdate = false;
                if ((Style & SFML.Graphics.Text.Styles.Underlined) != 0)
                {
                    if (Font.OutlineThickness == 0)
                        underline.FillColor = Color;
                    else
                    {
                        underline.FillColor = Color.Transparent;
                        underline.OutlineColor = Color;
                        underline.OutlineThickness = Font.OutlineThickness;
                    }
                    underline.Size = new SFML.System.Vector2f(FindCharacterPos(String.Length).X, Font.UnderlineThickness);
                    underline.Position = new SFML.System.Vector2f(0, Font.UnderlinePosition);
                }
                if ((Style & SFML.Graphics.Text.Styles.StrikeThrough) != 0)
                {
                    if (Font.OutlineThickness == 0)
                        strikeThrough.FillColor = Color;
                    else
                    {
                        strikeThrough.FillColor = Color.Transparent;
                        strikeThrough.OutlineColor = Color;
                        strikeThrough.OutlineThickness = Font.OutlineThickness;
                    }
                    strikeThrough.Size = new SFML.System.Vector2f(FindCharacterPos(String.Length).X, Font.UnderlineThickness);
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
            states.Texture = Font.Texture;
            target.Draw(buffer, PrimitiveType.Quads, states);
            if ((Style & SFML.Graphics.Text.Styles.StrikeThrough) != 0)
                target.Draw(strikeThrough, states);
            if ((Style & SFML.Graphics.Text.Styles.Underlined) != 0)
                target.Draw(underline, states);
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

                    offset.X += glyphs[i].Advance;
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
        public SFML.System.Vector2f FindCharacterPos(int pos)
        {
            Update();
            SFML.System.Vector2f offset = new SFML.System.Vector2f();
            for(int i = 0;i<pos;i++)
            {
                offset.X += glyphs[i].Advance;
                if (String[i] == '\n')
                {
                    offset.X = 0;
                    offset.Y += Font.LineSpacing;
                }
            }
            return offset;
        }
    }
}
