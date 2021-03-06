﻿using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace FC_UI.Controls
{
    [ToolboxBitmap(typeof(RichTextBox))]
    [Description("Обеспечивает дополнительные возможности ввода и редактирования текста (например, форматирование символов и абзацев).")]
    public partial class FRichTextBox : UserControl
    {
        #region ПЕРЕМЕННЫЕ
        private float h = 0;
        private Rectangle rectangle_region = new Rectangle();
        private GraphicsPath graphicsPath = new GraphicsPath();
        private readonly StringFormat stringFormat = new StringFormat();
        private Size size_frichtextbox = new Size();
        private readonly RichTextBox richTextBox = new RichTextBox();
        public enum Style
        {
            Default,
            Custom,
            Random
        }
        #endregion

        #region НАСТРОЙКИ
        public delegate void EventHandler();
        [Category("FC_UI")]
        [Description("Событие возникает, когда в Control изменяется значение свойства Text.")]
        public event EventHandler TextChanged = delegate { };

        [Category("FRichTextBox")]
        [Description("Текст на кнопке")]
        [DefaultValue("Text")]
        public string TextButton
        {
            get => richTextBox.Text;
            set
            {
                richTextBox.Text = value;
                TextChanged();
            }
        }
        //
        private bool tmp_rgb_status;
        [Category("FRichTextBox")]
        [Description("Вкл/Выкл RGB")]
        public bool RGB
        {
            get => tmp_rgb_status;
            set
            {
                tmp_rgb_status = value;

                if (tmp_rgb_status == true)
                {
                    timer_rgb.Stop();
                    if (!DrawEngine.timer_global_rgb.Enabled)
                    {
                        timer_rgb.Tick += (Sender, EventArgs) =>
                        {
                            h += 4;
                            if (h >= 360) h = 0;
                            Refresh();
                        };
                        timer_rgb.Start();
                    }
                }
                else
                {
                    timer_rgb.Stop();
                    Refresh();
                }
            }
        }
        //
        private bool tmp_rounding_status;
        [Category("FRichTextBox")]
        [Description("Вкл/Выкл закругления кнопки")]
        public bool Rounding
        {
            get => tmp_rounding_status;
            set
            {
                tmp_rounding_status = value;
                Refresh();
            }
        }
        //
        private int tmp_rounding_int;
        [Category("FRichTextBox")]
        [Description("Закругление в процентах")]
        public int RoundingInt
        {
            get => tmp_rounding_int;
            set
            {
                if (value >= 0 && value <= 100)
                {
                    tmp_rounding_int = value;
                    Refresh();
                }
            }
        }
        //
        private bool tmp_background_pen;
        [Category("BorderStyle")]
        [Description("Вкл/Выкл обводки")]
        public bool BackgroundPen
        {
            get => tmp_background_pen;
            set
            {
                tmp_background_pen = value;
                OnSizeChanged(null);
                Refresh();
            }
        }
        //
        private float tmp_background_width_pen;
        [Category("BorderStyle")]
        [Description("Размер обводки")]
        public float Background_WidthPen
        {
            get => tmp_background_width_pen;
            set
            {
                tmp_background_width_pen = value;
                OnSizeChanged(null);
                Refresh();
            }
        }
        //
        private Color tmp_color_background_pen;
        [Category("BorderStyle")]
        [Description("Цвет обводки")]
        public Color ColorBackground_Pen
        {
            get => tmp_color_background_pen;
            set
            {
                tmp_color_background_pen = value;
                Refresh();
            }
        }
        //
        private Color tmp_color_background;
        [Category("FRichTextBox")]
        [Description("Цвет фона")]
        public Color ColorBackground
        {
            get => tmp_color_background;
            set
            {
                if (value != Color.Transparent && value.A >= 255)
                {
                    tmp_color_background = value;
                    Refresh();
                }
            }
        }
        //
        private readonly Timer timer_rgb = new Timer() { Interval = 300 };
        [Category("Timers")]
        [Description("Скорость обновления RGB режима (действует перерисовка)")]
        public int Timer_RGB
        {
            get => timer_rgb.Interval;
            set => timer_rgb.Interval = value;
        }
        //
        private bool tmp_lighting;
        [Category("Lighting")]
        [Description("Вкл/Выкл подсветку")]
        public bool Lighting
        {
            get => tmp_lighting;
            set
            {
                tmp_lighting = value;
                OnSizeChanged(null);
                Refresh();
            }
        }
        //
        private Color tmp_color_lighting;
        [Category("Lighting")]
        [Description("Цвет подсветки / тени")]
        public Color ColorLighting
        {
            get => tmp_color_lighting;
            set
            {
                tmp_color_lighting = value;
                Refresh();
            }
        }
        //
        private int tmp_alpha;
        [Category("Lighting")]
        [Description("")]
        public int Alpha
        {
            get => tmp_alpha;
            set
            {
                tmp_alpha = value;
                Refresh();
            }
        }
        //
        private int tmp_pen_width;
        [Category("Lighting")]
        [Description("")]
        public int PenWidth
        {
            get => tmp_pen_width;
            set
            {
                tmp_pen_width = value;
                OnSizeChanged(null);
                Refresh();
            }
        }
        //
        private bool tmp_lineargradient_pen_status;
        [Category("LinearGradient")]
        [Description("Вкл/Выкл градиент обводки")]
        public bool LinearGradientPen
        {
            get => tmp_lineargradient_pen_status;
            set
            {
                tmp_lineargradient_pen_status = value;
                Refresh();
            }
        }
        //
        private Color tmp_color_1_for_gradient_pen;
        [Category("LinearGradient")]
        [Description("Цвет №1 для градиента обводки")]
        public Color ColorPen_1
        {
            get => tmp_color_1_for_gradient_pen;
            set
            {
                tmp_color_1_for_gradient_pen = value;
                Refresh();
            }
        }
        //
        private Color tmp_color_2_for_gradient_pen;
        [Category("LinearGradient")]
        [Description("Цвет №2 для градиента обводки")]
        public Color ColorPen_2
        {
            get => tmp_color_2_for_gradient_pen;
            set
            {
                tmp_color_2_for_gradient_pen = value;
                Refresh();
            }
        }
        //
        private SmoothingMode tmp_smoothing_mode;
        [Category("FRichTextBox")]
        [Description("Режим <graphics.SmoothingMode>")]
        public SmoothingMode SmoothingMode
        {
            get => tmp_smoothing_mode;
            set
            {
                if (value != SmoothingMode.Invalid) tmp_smoothing_mode = value;
                Refresh();
            }
        }
        //
        private TextRenderingHint tmp_text_rendering_hint;
        [Category("FRichTextBox")]
        [Description("Режим <graphics.TextRenderingHint>")]
        public TextRenderingHint TextRenderingHint
        {
            get => tmp_text_rendering_hint;
            set
            {
                tmp_text_rendering_hint = value;
                Refresh();
            }
        }
        //
        private Style tmp_frichtextbox_style = Style.Default;
        [Category("FRichTextBox")]
        [Description("Стиль кнопки")]
        public Style FRichTextBoxStyle
        {
            get => tmp_frichtextbox_style;
            set
            {
                tmp_frichtextbox_style = value;
                switch (tmp_frichtextbox_style)
                {
                    case Style.Default:
                        Size = new Size(150, 130);
                        BackColor = Color.Transparent;
                        ForeColor = Color.FromArgb(245, 245, 245);
                        TextButton = "FRichTextBox";
                        RGB = false;
                        Rounding = true;
                        RoundingInt = 60;
                        BackgroundPen = true;
                        Background_WidthPen = 3F;
                        ColorBackground_Pen = Color.FromArgb(29, 200, 238);
                        ColorBackground = Color.FromArgb(37, 52, 68);
                        Timer_RGB = 300;
                        Lighting = false;
                        ColorLighting = Color.FromArgb(29, 200, 238);
                        Alpha = 20;
                        PenWidth = 15;
                        LinearGradientPen = false;
                        ColorPen_1 = Color.FromArgb(29, 200, 238);
                        ColorPen_2 = Color.FromArgb(37, 52, 68);
                        SmoothingMode = SmoothingMode.HighQuality;
                        TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                        Font = HelpEngine.GetDefaultFont();
                        break;
                    case Style.Custom:
                        break;
                    case Style.Random:
                        HelpEngine.GetRandom random = new HelpEngine.GetRandom();
                        ColorBackground = random.ColorArgb();
                        Rounding = random.Bool();
                        if (Rounding) RoundingInt = random.Int(5, 90);
                        BackgroundPen = random.Bool();
                        if (BackgroundPen)
                        {
                            Background_WidthPen = random.Float(1, 3);
                            ColorBackground_Pen = random.ColorArgb(random.Int(0, 255));
                        }
                        Lighting = random.Bool();
                        if (Lighting) ColorLighting = random.ColorArgb();
                        LinearGradientPen = random.Bool();
                        if (LinearGradientPen)
                        {
                            ColorPen_1 = random.ColorArgb();
                            ColorPen_2 = random.ColorArgb();
                        }
                        break;
                }
                Refresh();
            }
        }
        #endregion

        #region ЗАГРУЗКА
        public FRichTextBox()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.ResizeRedraw |
            ControlStyles.Selectable | ControlStyles.SupportsTransparentBackColor | ControlStyles.StandardDoubleClick, true);
            DoubleBuffered = true;

            Tag = "FC_UI";
            FRichTextBoxStyle = Style.Default;
            FRichTextBoxStyle = Style.Custom;

            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;

            richTextBox.Text = "Text";
            Update_RichTextBox(false);
            Controls.Add(richTextBox);

            OnSizeChanged(null);
        }
        public void Update_RichTextBox(bool Visible)
        {
            richTextBox.Visible = Visible;
            richTextBox.Size = new Size(
                (int)(size_frichtextbox.Width - RoundingInt / 2 - Background_WidthPen / 2),
                (int)(size_frichtextbox.Height - RoundingInt / 2 - Background_WidthPen / 2));
            richTextBox.Location = new Point((Width / 2) - (richTextBox.Size.Width / 2), (Height / 2) - (richTextBox.Size.Height / 2));
            if (ColorBackground.Name != "Transparent")
            {
                richTextBox.BackColor = ColorBackground;
            }
            richTextBox.ForeColor = Color.WhiteSmoke;
            richTextBox.BorderStyle = BorderStyle.None;
            richTextBox.Font = Font;
            richTextBox.ScrollBars = RichTextBoxScrollBars.None;
            richTextBox.MaxLength = 10000;
        }
        #endregion

        #region СОБЫТИЯ
        protected override void OnPaint(PaintEventArgs e)
        {
            try
            {
                Settings_Load(e.Graphics);
                Draw_Background(e.Graphics);

                Update_RichTextBox(true);
            }
            catch (Exception er) { HelpEngine.MSB_Error($"[{Name}] Ошибка: \n{er}"); }
        }
        protected override void OnSizeChanged(EventArgs e)
        {
            int tmp = (int)((BackgroundPen ? Background_WidthPen : 0) + (Lighting ? (PenWidth) / 4 : 0));
            size_frichtextbox = new Size(Width - tmp * 2, Height - tmp * 2);
            rectangle_region = new Rectangle(tmp, tmp, size_frichtextbox.Width, size_frichtextbox.Height);
        }
        #endregion

        #region МЕТОДЫ РИСОВАНИЯ
        private void Settings_Load(Graphics graphics)
        {
            BackColor = Color.Transparent;

            graphics.SmoothingMode = SmoothingMode;
            graphics.TextRenderingHint = TextRenderingHint;
        }
        private void Draw_Background(Graphics graphics_form)
        {
            float roundingValue = 0.1F;
            void BaseLoading()
            {
                //Закругление
                if (Rounding && RoundingInt > 0)
                {
                    roundingValue = Height / 100F * RoundingInt;
                }
                //RoundedRectangle
                graphicsPath = DrawEngine.RoundedRectangle(rectangle_region, roundingValue);

                //Region
                Region = new Region(DrawEngine.RoundedRectangle(new Rectangle(
                0, 0,
                Width, Height),
                roundingValue));
            }
            Bitmap Layer_1()
            {
                Bitmap bitmap = new Bitmap(Width, Height);
                Graphics graphics = HelpEngine.GetGraphics(ref bitmap, SmoothingMode, TextRenderingHint);

                //Тень
                if (Lighting) DrawEngine.DrawBlurred(graphics, ColorLighting, DrawEngine.RoundedRectangle(rectangle_region, roundingValue), Alpha, PenWidth);

                //Обводка фона
                if (Background_WidthPen != 0 && BackgroundPen == true)
                {
                    Pen pen;
                    if (LinearGradientPen) pen = new Pen(new LinearGradientBrush(rectangle_region, ColorPen_1, ColorPen_2, 360), Background_WidthPen);
                    else pen = new Pen(RGB ? DrawEngine.HSV_To_RGB(h, 1f, 1f) : ColorBackground_Pen, Background_WidthPen);
                    pen.LineJoin = LineJoin.Round;
                    pen.DashCap = DashCap.Round;
                    graphics.DrawPath(pen, graphicsPath);
                }

                return bitmap;
            }
            Bitmap Layer_2()
            {
                Bitmap bitmap = new Bitmap(Width, Height);
                Graphics graphics = HelpEngine.GetGraphics(ref bitmap, SmoothingMode, TextRenderingHint);

                //Region_Clip
                graphics.Clip = new Region(DrawEngine.RoundedRectangle(new Rectangle(
                    rectangle_region.X - (int)(2 + Background_WidthPen),
                    rectangle_region.Y - (int)(2 + Background_WidthPen),
                    rectangle_region.Width + (int)(2 + Background_WidthPen) * 2,
                    rectangle_region.Height + (int)(2 + Background_WidthPen) * 2), Rounding ? roundingValue : 0.1F));

                //Фон
                graphics.FillPath(new SolidBrush(ColorBackground), graphicsPath);

                return bitmap;
            }

            BaseLoading();
            graphics_form.DrawImage(Layer_1(), new PointF(0, 0));
            graphics_form.DrawImage(Layer_2(), new PointF(0, 0));
        }
        #endregion
    }
}
