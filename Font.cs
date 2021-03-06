﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WGP.TEXT
{
    /// <summary>
    /// Similar to the SFML font, except it may crash ONLY at the creation, not when requesting a glyph or other methods. So it crashes less than the original and not in-game, but takes more memory.
    /// </summary>
    public class Font
    {
        private SFML.Graphics.Font InternFont;
        private SFML.Graphics.Glyph[] glyphs;
        private SFML.Graphics.Glyph[] boldGlyphs;
        /// <summary>
        /// Gets the font infos.
        /// </summary>
        public SFML.Graphics.Font.Info Info { get; private set; }
        /// <summary>
        /// Gets the line spacing.
        /// </summary>
        public float LineSpacing { get; private set; }
        /// <summary>
        /// Gets the underline position of the font.
        /// </summary>
        public float UnderlinePosition { get; private set; }
        /// <summary>
        /// Gets the underline thickness of the line.
        /// </summary>
        public float UnderlineThickness { get; private set; }
        /// <summary>
        /// Gets the underline thickness of the line.
        /// </summary>
        public float OutlineThickness { get; private set; }
        /// <summary>
        /// Gets the character size defined at the creation (not modifiable, sadly).
        /// </summary>
        public uint CharSize { get; }
        /// <summary>
        /// Gets the font texture.
        /// </summary>
        public SFML.Graphics.Texture Texture { get; private set; }
        /// <summary>
        /// Gets a specific glyph. The glyphs are already stored at the beginning, so it won't crash.
        /// </summary>
        /// <param name="code">ASCII code.</param>
        /// <param name="bold">True if the glyph is bold.</param>
        /// <returns>Requested glyph.</returns>
        public SFML.Graphics.Glyph GetGlyph(char code, bool bold)
        {
            if (bold)
                return boldGlyphs[code];
            else
                return glyphs[code];
        }
        private void Init()
        {
            Info = InternFont.GetInfo();
            LineSpacing = InternFont.GetLineSpacing(CharSize);
            UnderlinePosition = InternFont.GetUnderlinePosition(CharSize);
            UnderlineThickness = InternFont.GetUnderlineThickness(CharSize);
        }
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="path">Path of the file.</param>
        /// <param name="charSize">Character size.</param>
        /// <param name="outline">Outline thickness (Optional).</param>
        public Font(string path, uint charSize, float outline = 0)
        {
            InternFont = new SFML.Graphics.Font(path);
            CharSize = charSize;
            glyphs = new SFML.Graphics.Glyph[256];
            boldGlyphs = new SFML.Graphics.Glyph[256];
            for (char i = (char)0; i < 256; i++)
                glyphs[i] = InternFont.GetGlyph(i, CharSize, false, outline);
            for (char i = (char)0; i < 256; i++)
                boldGlyphs[i] = InternFont.GetGlyph(i, CharSize, true, outline);
            Texture = new SFML.Graphics.Texture(InternFont.GetTexture(charSize));
            OutlineThickness = outline;
            Init();
        }
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="stream">Input stream.</param>
        /// <param name="charSize">Character size.</param>
        /// <param name="outline">Outline thickness (Optional).</param>
        public Font(System.IO.Stream stream, uint charSize, float outline = 0)
        {
            InternFont = new SFML.Graphics.Font(stream);
            CharSize = charSize;
            glyphs = new SFML.Graphics.Glyph[256];
            boldGlyphs = new SFML.Graphics.Glyph[256];
            for (char i = (char)0; i < 256; i++)
                glyphs[i] = InternFont.GetGlyph(i, CharSize, false, outline);
            for (char i = (char)0; i < 256; i++)
                boldGlyphs[i] = InternFont.GetGlyph(i, CharSize, true, outline);
            Texture = new SFML.Graphics.Texture(InternFont.GetTexture(charSize));
            OutlineThickness = outline;
            Init();
        }
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="data">Input data.</param>
        /// <param name="charSize">Character size.</param>
        /// <param name="outline">Outline thickness (Optional).</param>
        public Font(byte[] data, uint charSize, float outline = 0)
        {
            InternFont = new SFML.Graphics.Font(data);
            CharSize = charSize;
            glyphs = new SFML.Graphics.Glyph[256];
            boldGlyphs = new SFML.Graphics.Glyph[256];
            for (char i = (char)0; i < 256; i++)
                glyphs[i] = InternFont.GetGlyph(i, CharSize, false, outline);
            for (char i = (char)0; i < 256; i++)
                boldGlyphs[i] = InternFont.GetGlyph(i, CharSize, true, outline);
            Texture = new SFML.Graphics.Texture(InternFont.GetTexture(charSize));
            OutlineThickness = outline;
            Init();
        }
    }
}
