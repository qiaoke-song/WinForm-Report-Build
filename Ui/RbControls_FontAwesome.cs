using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

public class RbControls_FontAwesome
{
    public class Properties
    {
        public int Size { get; set; }
        public Point Location { get; set; }
        public Color ForeColor { get; set; }
        public Color BackColor { get; set; }
        public Color BorderColor { get; set; }
        public bool ShowBorder { get; set; }
        public Type Type { get; set; }
        private Properties()
        {
            Size = 32;
            Location = new Point(0, 0);
            ForeColor = Color.Black;
            BackColor = Color.Transparent;
            BorderColor = Color.Gray;
            ShowBorder = false;
        }

        public Properties(Type type)
        {
            Size = Default.Size;
            Location = Default.Location;
            ForeColor = Default.ForeColor;
            BackColor = Default.BackColor;
            BorderColor = Default.BorderColor;
            ShowBorder = Default.ShowBorder;
            Type = type;
        }

        public static Properties Get(Type type = Type.None)
        {
            var props = Default;
            props.Type = type;
            return props;
        }

        private static Properties _default;
        public static Properties Default
        {
            get
            {
                if (_default == null)
                {
                    _default = new Properties();
                }
                return _default;
            }
            internal set
            {
                _default = value;
            }
        }

    }

    private PrivateFontCollection _fonts = new PrivateFontCollection();
    private const string FONT_FILE_NAME = "FontAwesome-webfont.ttf";

    #region statics
    private static RbControls_FontAwesome _instance;

    public static void Initialize()
    {
        if (_instance == null)
        {
            _instance = new RbControls_FontAwesome();
        }
    }

    public static RbControls_FontAwesome Instance
    {
        get
        {
            if (_instance == null)
            {
                Initialize();
            }
            return _instance;
        }
    }

    public static Properties DefaultProperties { get { return Properties.Default; } }

    #endregion

    #region public methods

    public void SetDefaultProperties(Properties props)
    {
        Properties.Default = props;
    }

    public Icon GetIcon(Type type, Properties props = null)
    {
        if (props == null)
        {
            props = Properties.Default;
        }
        props.Type = type;
        return GetIcon(props);
    }

    public Icon GetIcon(Properties props)
    {
        var img = GetImage(props);
        return Icon.FromHandle(img.GetHicon());
    }

    public Bitmap GetImage(string name, Properties props = null)
    {
        if (props == null)
        {
            props = Properties.Default;
        }
        if (props.Type != Type.None)
        {
            return GetImage(Properties.Get(ParseType(name)));
        }
        return null;
    }

    public Bitmap GetImage(Type type, Properties props = null)
    {
        if (props == null)
        {
            props = Properties.Default;
        }
        props.Type = type;
        return GetImage(props);
    }

    public Bitmap GetImage(Properties props)
    {
        return GetImageInternal(props);
    }

    #endregion

    #region private methods

    private RbControls_FontAwesome()
    {
        LoadFont();
    }

    private Bitmap GetImageInternal(Properties props)
    {
        var size = GetFontIconRealSize(props.Size, (int)props.Type);
        var bmpTemp = new Bitmap(size.Width, size.Height);
        using (Graphics g1 = Graphics.FromImage(bmpTemp))
        {
            g1.TextRenderingHint = TextRenderingHint.AntiAlias;
            g1.Clear(Color.Transparent);
            var font = GetIconFont(props.Size);
            if (font != null)
            {
                string character = char.ConvertFromUtf32((int)props.Type);
                var format = new StringFormat()
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center,
                    Trimming = StringTrimming.Character
                };

                g1.DrawString(character, font, new SolidBrush(props.ForeColor), 0, 0);
                g1.DrawImage(bmpTemp, 0, 0);
            }
        }

        var bmp = ResizeImage(bmpTemp, props);
        if (props.ShowBorder)
        {
            using (Graphics g2 = Graphics.FromImage(bmp))
            {
                var pen = new Pen(props.BorderColor, 1);
                var borderRect = new Rectangle(0, 0, (int)(props.Size - pen.Width), (int)(props.Size - pen.Width));
                g2.DrawRectangle(pen, borderRect);
            }
        }
        return bmp;
    }

    private Size GetFontIconRealSize(int size, int iconIndex)
    {
        var bmpTemp = new Bitmap(size, size);
        using (Graphics g1 = Graphics.FromImage(bmpTemp))
        {
            g1.TextRenderingHint = TextRenderingHint.AntiAlias;
            g1.PixelOffsetMode = PixelOffsetMode.HighQuality;
            var font = GetIconFont(size);
            if (font != null)
            {
                string character = char.ConvertFromUtf32(iconIndex);
                var format = new StringFormat()
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center,
                    Trimming = StringTrimming.Word
                };

                var sizeF = g1.MeasureString(character, font, new Point(0, 0), format);
                return sizeF.ToSize();
            }
        }
        return new Size(size, size);
    }

    private Bitmap ResizeImage(Bitmap imgToResize, Properties props)
    {
        var srcWidth = imgToResize.Width;
        var srcHeight = imgToResize.Height;

        float ratio = (srcWidth > srcHeight) ? (srcWidth / (float)srcHeight) : (srcHeight / (float)srcWidth);

        var dstWidth = (int)Math.Ceiling(srcWidth / ratio);
        var dstHeight = (int)Math.Ceiling(srcHeight / ratio);

        var x = (int)Math.Round((props.Size - dstWidth) / 2f, 0);
        var y = (int)(1 + Math.Round((props.Size - dstHeight) / 2f, 0));

        Bitmap b = new Bitmap(props.Size + props.Location.X, props.Size + props.Location.Y);
        using (Graphics g = Graphics.FromImage((Image)b))
        {
            g.Clear(props.BackColor);
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.DrawImage(imgToResize, x + props.Location.X, y + props.Location.Y, dstWidth, dstHeight);
        }
        return b;
    }

    private void LoadFont()
    {
        if (System.IO.File.Exists(FONT_FILE_NAME))
        {
            _fonts.AddFontFile(FONT_FILE_NAME);
        }
    }

    private Font GetIconFont(int pixelSize)
    {
        var size = pixelSize / (16f / 12f);
        var font = new Font(_fonts.Families[0], size, FontStyle.Regular, GraphicsUnit.Point);
        return font;
    }

    public static Type ParseType(string name)
    {
        Type retval = Type.Empty;
        if (!string.IsNullOrEmpty(name))
        {
            retval = (Type)Enum.Parse(typeof(Type), name);
        }
        return retval;
    }
    #endregion

    #region IconNames Type Enum

    public enum Type
    {
        //not part of RbControls_FontAwesome, use this empty image
        None = 0x0,
        //alias to None
        Empty = 0x0,
        //-----
        //4.4, 500Px, 0xf26E
        Px500 = 0xf26e,
        //4.7, AddressBook, 0xf2B9
        AddressBook = 0xf2B9,
        //4.7, AddressBookO, 0xf2Ba
        AddressBookO = 0xf2Ba,
        //4.7, AddressCard, 0xf2Bb
        AddressCard = 0xf2Bb,
        //4.7, AddressCardO, 0xf2Bc
        AddressCardO = 0xf2Bc,
        //Adjust, 0xf042
        Adjust = 0xf042,
        //Adn, 0xf170
        Adn = 0xf170,
        //AlignCenter, 0xf037
        AlignCenter = 0xf037,
        //AlignJustify, 0xf039
        AlignJustify = 0xf039,
        //AlignLeft, 0xf036
        AlignLeft = 0xf036,
        //AlignRight, 0xf038
        AlignRight = 0xf038,
        //4.4, Amazon, 0xf270
        Amazon = 0xf270,
        //Ambulance, 0xf0F9
        Ambulance = 0xf0F9,
        //4.6, AmericanSignLanguageInterpreting, 0xf2A3
        AmericanSignLanguageInterpreting = 0xf2A3,
        //Anchor, 0xf13D
        Anchor = 0xf13D,
        //Android, 0xf17B
        Android = 0xf17B,
        //4.2, Angellist, 0xf209
        Angellist = 0xf209,
        //AngleDoubleDown, 0xf103
        AngleDoubleDown = 0xf103,
        //AngleDoubleLeft, 0xf100
        AngleDoubleLeft = 0xf100,
        //AngleDoubleRight, 0xf101
        AngleDoubleRight = 0xf101,
        //AngleDoubleUp, 0xf102
        AngleDoubleUp = 0xf102,
        //AngleDown, 0xf107
        AngleDown = 0xf107,
        //AngleLeft, 0xf104
        AngleLeft = 0xf104,
        //AngleRight, 0xf105
        AngleRight = 0xf105,
        //AngleUp, 0xf106
        AngleUp = 0xf106,
        //Apple, 0xf179
        Apple = 0xf179,
        //Archive, 0xf187
        Archive = 0xf187,
        //4.2, AreaChart, 0xf1Fe
        AreaChart = 0xf1Fe,
        //ArrowCircleDown, 0xf0Ab
        ArrowCircleDown = 0xf0Ab,
        //ArrowCircleLeft, 0xf0A8
        ArrowCircleLeft = 0xf0A8,
        //ArrowCircleODown, 0xf01A
        ArrowCircleODown = 0xf01A,
        //4.0, ArrowCircleOLeft, 0xf190
        ArrowCircleOLeft = 0xf190,
        //4.0, ArrowCircleORight, 0xf18E
        ArrowCircleORight = 0xf18E,
        //ArrowCircleOUp, 0xf01B
        ArrowCircleOUp = 0xf01B,
        //ArrowCircleRight, 0xf0A9
        ArrowCircleRight = 0xf0A9,
        //ArrowCircleUp, 0xf0Aa
        ArrowCircleUp = 0xf0Aa,
        //ArrowDown, 0xf063
        ArrowDown = 0xf063,
        //ArrowLeft, 0xf060
        ArrowLeft = 0xf060,
        //ArrowRight, 0xf061
        ArrowRight = 0xf061,
        //ArrowUp, 0xf062
        ArrowUp = 0xf062,
        //Arrows, 0xf047
        Arrows = 0xf047,
        //ArrowsAlt, 0xf0B2
        ArrowsAlt = 0xf0B2,
        //ArrowsH, 0xf07E
        ArrowsH = 0xf07E,
        //ArrowsV, 0xf07D
        ArrowsV = 0xf07D,
        //4.6, AslInterpreting, 0xf2A3
        AslInterpreting = 0xf2A3,
        //4.6, AssistiveListeningSystems, 0xf2A2
        AssistiveListeningSystems = 0xf2A2,
        //Asterisk, 0xf069
        Asterisk = 0xf069,
        //4.2, At, 0xf1Fa
        At = 0xf1Fa,
        //4.6, AudioDescription, 0xf29E
        AudioDescription = 0xf29E,
        //4.1, Automobile, 0xf1B9
        Automobile = 0xf1B9,
        //Backward, 0xf04A
        Backward = 0xf04A,
        //4.4, BalanceScale, 0xf24E
        BalanceScale = 0xf24E,
        //Ban, 0xf05E
        Ban = 0xf05E,
        //4.7, Bandcamp, 0xf2D5
        Bandcamp = 0xf2D5,
        //4.1, Bank, 0xf19C
        Bank = 0xf19C,
        //BarChart, 0xf080
        BarChart = 0xf080,
        //BarChartO, 0xf080
        BarChartO = 0xf080,
        //Barcode, 0xf02A
        Barcode = 0xf02A,
        //Bars, 0xf0C9
        Bars = 0xf0C9,
        //4.7, Bath, 0xf2Cd
        Bath = 0xf2Cd,
        //4.7, Bathtub, 0xf2Cd
        Bathtub = 0xf2Cd,
        //4.4, Battery, 0xf240
        Battery = 0xf240,
        //4.4, Battery0, 0xf244
        Battery0 = 0xf244,
        //4.4, Battery1, 0xf243
        Battery1 = 0xf243,
        //4.4, Battery2, 0xf242
        Battery2 = 0xf242,
        //4.4, Battery3, 0xf241
        Battery3 = 0xf241,
        //4.4, Battery4, 0xf240
        Battery4 = 0xf240,
        //4.4, BatteryEmpty, 0xf244
        BatteryEmpty = 0xf244,
        //4.4, BatteryFull, 0xf240
        BatteryFull = 0xf240,
        //4.4, BatteryHalf, 0xf242
        BatteryHalf = 0xf242,
        //4.4, BatteryQuarter, 0xf243
        BatteryQuarter = 0xf243,
        //4.4, BatteryThreeQuarters, 0xf241
        BatteryThreeQuarters = 0xf241,
        //4.3, Bed, 0xf236
        Bed = 0xf236,
        //Beer, 0xf0Fc
        Beer = 0xf0Fc,
        //4.1, Behance, 0xf1B4
        Behance = 0xf1B4,
        //4.1, BehanceSquare, 0xf1B5
        BehanceSquare = 0xf1B5,
        //Bell, 0xf0F3
        Bell = 0xf0F3,
        //BellO, 0xf0A2
        BellO = 0xf0A2,
        //4.2, BellSlash, 0xf1F6
        BellSlash = 0xf1F6,
        //4.2, BellSlashO, 0xf1F7
        BellSlashO = 0xf1F7,
        //4.2, Bicycle, 0xf206
        Bicycle = 0xf206,
        //4.2, Binoculars, 0xf1E5
        Binoculars = 0xf1E5,
        //4.2, BirthdayCake, 0xf1Fd
        BirthdayCake = 0xf1Fd,
        //Bitbucket, 0xf171
        Bitbucket = 0xf171,
        //BitbucketSquare, 0xf172
        BitbucketSquare = 0xf172,
        //Bitcoin, 0xf15A
        Bitcoin = 0xf15A,
        //4.4, BlackTie, 0xf27E
        BlackTie = 0xf27E,
        //4.6, Blind, 0xf29D
        Blind = 0xf29D,
        //4.5, Bluetooth, 0xf293
        Bluetooth = 0xf293,
        //4.5, BluetoothB, 0xf294
        BluetoothB = 0xf294,
        //Bold, 0xf032
        Bold = 0xf032,
        //Bolt, 0xf0E7
        Bolt = 0xf0E7,
        //4.1, Bomb, 0xf1E2
        Bomb = 0xf1E2,
        //Book, 0xf02D
        Book = 0xf02D,
        //Bookmark, 0xf02E
        Bookmark = 0xf02E,
        //BookmarkO, 0xf097
        BookmarkO = 0xf097,
        //4.6, Braille, 0xf2A1
        Braille = 0xf2A1,
        //Briefcase, 0xf0B1
        Briefcase = 0xf0B1,
        //Btc, 0xf15A
        Btc = 0xf15A,
        //Bug, 0xf188
        Bug = 0xf188,
        //4.1, Building, 0xf1Ad
        Building = 0xf1Ad,
        //BuildingO, 0xf0F7
        BuildingO = 0xf0F7,
        //Bullhorn, 0xf0A1
        Bullhorn = 0xf0A1,
        //Bullseye, 0xf140
        Bullseye = 0xf140,
        //4.2, Bus, 0xf207
        Bus = 0xf207,
        //4.3, Buysellads, 0xf20D
        Buysellads = 0xf20D,
        //4.1, Cab, 0xf1Ba
        Cab = 0xf1Ba,
        //4.2, Calculator, 0xf1Ec
        Calculator = 0xf1Ec,
        //Calendar, 0xf073
        Calendar = 0xf073,
        //4.4, CalendarCheckO, 0xf274
        CalendarCheckO = 0xf274,
        //4.4, CalendarMinusO, 0xf272
        CalendarMinusO = 0xf272,
        //CalendarO, 0xf133
        CalendarO = 0xf133,
        //4.4, CalendarPlusO, 0xf271
        CalendarPlusO = 0xf271,
        //4.4, CalendarTimesO, 0xf273
        CalendarTimesO = 0xf273,
        //Camera, 0xf030
        Camera = 0xf030,
        //CameraRetro, 0xf083
        CameraRetro = 0xf083,
        //4.1, Car, 0xf1B9
        Car = 0xf1B9,
        //CaretDown, 0xf0D7
        CaretDown = 0xf0D7,
        //CaretLeft, 0xf0D9
        CaretLeft = 0xf0D9,
        //CaretRight, 0xf0Da
        CaretRight = 0xf0Da,
        //CaretSquareODown, 0xf150
        CaretSquareODown = 0xf150,
        //4.0, CaretSquareOLeft, 0xf191
        CaretSquareOLeft = 0xf191,
        //CaretSquareORight, 0xf152
        CaretSquareORight = 0xf152,
        //CaretSquareOUp, 0xf151
        CaretSquareOUp = 0xf151,
        //CaretUp, 0xf0D8
        CaretUp = 0xf0D8,
        //4.3, CartArrowDown, 0xf218
        CartArrowDown = 0xf218,
        //4.3, CartPlus, 0xf217
        CartPlus = 0xf217,
        //4.2, Cc, 0xf20A
        Cc = 0xf20A,
        //4.2, CcAmex, 0xf1F3
        CcAmex = 0xf1F3,
        //4.4, CcDinersClub, 0xf24C
        CcDinersClub = 0xf24C,
        //4.2, CcDiscover, 0xf1F2
        CcDiscover = 0xf1F2,
        //4.4, CcJcb, 0xf24B
        CcJcb = 0xf24B,
        //4.2, CcMastercard, 0xf1F1
        CcMastercard = 0xf1F1,
        //4.2, CcPaypal, 0xf1F4
        CcPaypal = 0xf1F4,
        //4.2, CcStripe, 0xf1F5
        CcStripe = 0xf1F5,
        //4.2, CcVisa, 0xf1F0
        CcVisa = 0xf1F0,
        //Certificate, 0xf0A3
        Certificate = 0xf0A3,
        //Chain, 0xf0C1
        Chain = 0xf0C1,
        //ChainBroken, 0xf127
        ChainBroken = 0xf127,
        //Check, 0xf00C
        Check = 0xf00C,
        //CheckCircle, 0xf058
        CheckCircle = 0xf058,
        //CheckCircleO, 0xf05D
        CheckCircleO = 0xf05D,
        //CheckSquare, 0xf14A
        CheckSquare = 0xf14A,
        //CheckSquareO, 0xf046
        CheckSquareO = 0xf046,
        //ChevronCircleDown, 0xf13A
        ChevronCircleDown = 0xf13A,
        //ChevronCircleLeft, 0xf137
        ChevronCircleLeft = 0xf137,
        //ChevronCircleRight, 0xf138
        ChevronCircleRight = 0xf138,
        //ChevronCircleUp, 0xf139
        ChevronCircleUp = 0xf139,
        //ChevronDown, 0xf078
        ChevronDown = 0xf078,
        //ChevronLeft, 0xf053
        ChevronLeft = 0xf053,
        //ChevronRight, 0xf054
        ChevronRight = 0xf054,
        //ChevronUp, 0xf077
        ChevronUp = 0xf077,
        //4.1, Child, 0xf1Ae
        Child = 0xf1Ae,
        //4.4, Chrome, 0xf268
        Chrome = 0xf268,
        //Circle, 0xf111
        Circle = 0xf111,
        //CircleO, 0xf10C
        CircleO = 0xf10C,
        //4.1, CircleONotch, 0xf1Ce
        CircleONotch = 0xf1Ce,
        //4.1, CircleThin, 0xf1Db
        CircleThin = 0xf1Db,
        //Clipboard, 0xf0Ea
        Clipboard = 0xf0Ea,
        //ClockO, 0xf017
        ClockO = 0xf017,
        //4.4, Clone, 0xf24D
        Clone = 0xf24D,
        //Close, 0xf00D
        Close = 0xf00D,
        //Cloud, 0xf0C2
        Cloud = 0xf0C2,
        //CloudDownload, 0xf0Ed
        CloudDownload = 0xf0Ed,
        //CloudUpload, 0xf0Ee
        CloudUpload = 0xf0Ee,
        //Cny, 0xf157
        Cny = 0xf157,
        //Code, 0xf121
        Code = 0xf121,
        //CodeFork, 0xf126
        CodeFork = 0xf126,
        //4.1, Codepen, 0xf1Cb
        Codepen = 0xf1Cb,
        //4.5, Codiepie, 0xf284
        Codiepie = 0xf284,
        //Coffee, 0xf0F4
        Coffee = 0xf0F4,
        //Cog, 0xf013
        Cog = 0xf013,
        //Cogs, 0xf085
        Cogs = 0xf085,
        //Columns, 0xf0Db
        Columns = 0xf0Db,
        //Comment, 0xf075
        Comment = 0xf075,
        //CommentO, 0xf0E5
        CommentO = 0xf0E5,
        //4.4, Commenting, 0xf27A
        Commenting = 0xf27A,
        //4.4, CommentingO, 0xf27B
        CommentingO = 0xf27B,
        //Comments, 0xf086
        Comments = 0xf086,
        //CommentsO, 0xf0E6
        CommentsO = 0xf0E6,
        //Compass, 0xf14E
        Compass = 0xf14E,
        //Compress, 0xf066
        Compress = 0xf066,
        //4.3, Connectdevelop, 0xf20E
        Connectdevelop = 0xf20E,
        //4.4, Contao, 0xf26D
        Contao = 0xf26D,
        //Copy, 0xf0C5
        Copy = 0xf0C5,
        //4.2, Copyright, 0xf1F9
        Copyright = 0xf1F9,
        //4.4, CreativeCommons, 0xf25E
        CreativeCommons = 0xf25E,
        //CreditCard, 0xf09D
        CreditCard = 0xf09D,
        //4.5, CreditCardAlt, 0xf283
        CreditCardAlt = 0xf283,
        //Crop, 0xf125
        Crop = 0xf125,
        //Crosshairs, 0xf05B
        Crosshairs = 0xf05B,
        //Css3, 0xf13C
        Css3 = 0xf13C,
        //4.1, Cube, 0xf1B2
        Cube = 0xf1B2,
        //4.1, Cubes, 0xf1B3
        Cubes = 0xf1B3,
        //Cut, 0xf0C4
        Cut = 0xf0C4,
        //Cutlery, 0xf0F5
        Cutlery = 0xf0F5,
        //Dashboard, 0xf0E4
        Dashboard = 0xf0E4,
        //4.3, Dashcube, 0xf210
        Dashcube = 0xf210,
        //4.1, Database, 0xf1C0
        Database = 0xf1C0,
        //4.6, Deaf, 0xf2A4
        Deaf = 0xf2A4,
        //4.6, Deafness, 0xf2A4
        Deafness = 0xf2A4,
        //Dedent, 0xf03B
        Dedent = 0xf03B,
        //4.1, Delicious, 0xf1A5
        Delicious = 0xf1A5,
        //Desktop, 0xf108
        Desktop = 0xf108,
        //4.1, Deviantart, 0xf1Bd
        Deviantart = 0xf1Bd,
        //4.3, Diamond, 0xf219
        Diamond = 0xf219,
        //4.1, Digg, 0xf1A6
        Digg = 0xf1A6,
        //Dollar, 0xf155
        Dollar = 0xf155,
        //4.0, DotCircleO, 0xf192
        DotCircleO = 0xf192,
        //Download, 0xf019
        Download = 0xf019,
        //Dribbble, 0xf17D
        Dribbble = 0xf17D,
        //4.7, DriversLicense, 0xf2C2
        DriversLicense = 0xf2C2,
        //4.7, DriversLicenseO, 0xf2C3
        DriversLicenseO = 0xf2C3,
        //Dropbox, 0xf16B
        Dropbox = 0xf16B,
        //4.1, Drupal, 0xf1A9
        Drupal = 0xf1A9,
        //4.5, Edge, 0xf282
        Edge = 0xf282,
        //Edit, 0xf044
        Edit = 0xf044,
        //4.7, Eercast, 0xf2Da
        Eercast = 0xf2Da,
        //Eject, 0xf052
        Eject = 0xf052,
        //EllipsisH, 0xf141
        EllipsisH = 0xf141,
        //EllipsisV, 0xf142
        EllipsisV = 0xf142,
        //4.1, Empire, 0xf1D1
        Empire = 0xf1D1,
        //Envelope, 0xf0E0
        Envelope = 0xf0E0,
        //EnvelopeO, 0xf003
        EnvelopeO = 0xf003,
        //4.7, EnvelopeOpen, 0xf2B6
        EnvelopeOpen = 0xf2B6,
        //4.7, EnvelopeOpenO, 0xf2B7
        EnvelopeOpenO = 0xf2B7,
        //4.1, EnvelopeSquare, 0xf199
        EnvelopeSquare = 0xf199,
        //4.6, Envira, 0xf299
        Envira = 0xf299,
        //Eraser, 0xf12D
        Eraser = 0xf12D,
        //4.7, Etsy, 0xf2D7
        Etsy = 0xf2D7,
        //Eur, 0xf153
        Eur = 0xf153,
        //Euro, 0xf153
        Euro = 0xf153,
        //Exchange, 0xf0Ec
        Exchange = 0xf0Ec,
        //Exclamation, 0xf12A
        Exclamation = 0xf12A,
        //ExclamationCircle, 0xf06A
        ExclamationCircle = 0xf06A,
        //ExclamationTriangle, 0xf071
        ExclamationTriangle = 0xf071,
        //Expand, 0xf065
        Expand = 0xf065,
        //4.4, Expeditedssl, 0xf23E
        Expeditedssl = 0xf23E,
        //ExternalLink, 0xf08E
        ExternalLink = 0xf08E,
        //ExternalLinkSquare, 0xf14C
        ExternalLinkSquare = 0xf14C,
        //Eye, 0xf06E
        Eye = 0xf06E,
        //EyeSlash, 0xf070
        EyeSlash = 0xf070,
        //4.2, Eyedropper, 0xf1Fb
        Eyedropper = 0xf1Fb,
        //4.6, Fa, 0xf2B4
        Fa = 0xf2B4,
        //Facebook, 0xf09A
        Facebook = 0xf09A,
        //FacebookF, 0xf09A
        FacebookF = 0xf09A,
        //4.3, FacebookOfficial, 0xf230
        FacebookOfficial = 0xf230,
        //FacebookSquare, 0xf082
        FacebookSquare = 0xf082,
        //FastBackward, 0xf049
        FastBackward = 0xf049,
        //FastForward, 0xf050
        FastForward = 0xf050,
        //4.1, Fax, 0xf1Ac
        Fax = 0xf1Ac,
        //Feed, 0xf09E
        Feed = 0xf09E,
        //Female, 0xf182
        Female = 0xf182,
        //FighterJet, 0xf0Fb
        FighterJet = 0xf0Fb,
        //File, 0xf15B
        File = 0xf15B,
        //4.1, FileArchiveO, 0xf1C6
        FileArchiveO = 0xf1C6,
        //4.1, FileAudioO, 0xf1C7
        FileAudioO = 0xf1C7,
        //4.1, FileCodeO, 0xf1C9
        FileCodeO = 0xf1C9,
        //4.1, FileExcelO, 0xf1C3
        FileExcelO = 0xf1C3,
        //4.1, FileImageO, 0xf1C5
        FileImageO = 0xf1C5,
        //4.1, FileMovieO, 0xf1C8
        FileMovieO = 0xf1C8,
        //FileO, 0xf016
        FileO = 0xf016,
        //4.1, FilePdfO, 0xf1C1
        FilePdfO = 0xf1C1,
        //4.1, FilePhotoO, 0xf1C5
        FilePhotoO = 0xf1C5,
        //4.1, FilePictureO, 0xf1C5
        FilePictureO = 0xf1C5,
        //4.1, FilePowerpointO, 0xf1C4
        FilePowerpointO = 0xf1C4,
        //4.1, FileSoundO, 0xf1C7
        FileSoundO = 0xf1C7,
        //FileText, 0xf15C
        FileText = 0xf15C,
        //FileTextO, 0xf0F6
        FileTextO = 0xf0F6,
        //4.1, FileVideoO, 0xf1C8
        FileVideoO = 0xf1C8,
        //4.1, FileWordO, 0xf1C2
        FileWordO = 0xf1C2,
        //4.1, FileZipO, 0xf1C6
        FileZipO = 0xf1C6,
        //FilesO, 0xf0C5
        FilesO = 0xf0C5,
        //Film, 0xf008
        Film = 0xf008,
        //Filter, 0xf0B0
        Filter = 0xf0B0,
        //Fire, 0xf06D
        Fire = 0xf06D,
        //FireExtinguisher, 0xf134
        FireExtinguisher = 0xf134,
        //4.4, Firefox, 0xf269
        Firefox = 0xf269,
        //4.6, FirstOrder, 0xf2B0
        FirstOrder = 0xf2B0,
        //Flag, 0xf024
        Flag = 0xf024,
        //FlagCheckered, 0xf11E
        FlagCheckered = 0xf11E,
        //FlagO, 0xf11D
        FlagO = 0xf11D,
        //Flash, 0xf0E7
        Flash = 0xf0E7,
        //Flask, 0xf0C3
        Flask = 0xf0C3,
        //Flickr, 0xf16E
        Flickr = 0xf16E,
        //FloppyO, 0xf0C7
        FloppyO = 0xf0C7,
        //Folder, 0xf07B
        Folder = 0xf07B,
        //FolderO, 0xf114
        FolderO = 0xf114,
        //FolderOpen, 0xf07C
        FolderOpen = 0xf07C,
        //FolderOpenO, 0xf115
        FolderOpenO = 0xf115,
        //Font, 0xf031
        Font = 0xf031,
        //4.6, RbControls_FontAwesome, 0xf2B4
        RbControls_FontAwesome = 0xf2B4,
        //4.4, Fonticons, 0xf280
        Fonticons = 0xf280,
        //4.5, FortAwesome, 0xf286
        FortAwesome = 0xf286,
        //4.3, Forumbee, 0xf211
        Forumbee = 0xf211,
        //Forward, 0xf04E
        Forward = 0xf04E,
        //Foursquare, 0xf180
        Foursquare = 0xf180,
        //4.7, FreeCodeCamp, 0xf2C5
        FreeCodeCamp = 0xf2C5,
        //FrownO, 0xf119
        FrownO = 0xf119,
        //4.2, FutbolO, 0xf1E3
        FutbolO = 0xf1E3,
        //Gamepad, 0xf11B
        Gamepad = 0xf11B,
        //Gavel, 0xf0E3
        Gavel = 0xf0E3,
        //Gbp, 0xf154
        Gbp = 0xf154,
        //4.1, Ge, 0xf1D1
        Ge = 0xf1D1,
        //Gear, 0xf013
        Gear = 0xf013,
        //Gears, 0xf085
        Gears = 0xf085,
        //4.4, Genderless, 0xf22D
        Genderless = 0xf22D,
        //4.4, GetPocket, 0xf265
        GetPocket = 0xf265,
        //4.4, Gg, 0xf260
        Gg = 0xf260,
        //4.4, GgCircle, 0xf261
        GgCircle = 0xf261,
        //Gift, 0xf06B
        Gift = 0xf06B,
        //4.1, Git, 0xf1D3
        Git = 0xf1D3,
        //4.1, GitSquare, 0xf1D2
        GitSquare = 0xf1D2,
        //Github, 0xf09B
        Github = 0xf09B,
        //GithubAlt, 0xf113
        GithubAlt = 0xf113,
        //GithubSquare, 0xf092
        GithubSquare = 0xf092,
        //4.6, Gitlab, 0xf296
        Gitlab = 0xf296,
        //Gittip, 0xf184
        Gittip = 0xf184,
        //Glass, 0xf000
        Glass = 0xf000,
        //4.6, Glide, 0xf2A5
        Glide = 0xf2A5,
        //4.6, GlideG, 0xf2A6
        GlideG = 0xf2A6,
        //Globe, 0xf0Ac
        Globe = 0xf0Ac,
        //4.1, Google, 0xf1A0
        Google = 0xf1A0,
        //GooglePlus, 0xf0D5
        GooglePlus = 0xf0D5,
        //4.6, GooglePlusCircle, 0xf2B3
        GooglePlusCircle = 0xf2B3,
        //4.6, GooglePlusOfficial, 0xf2B3
        GooglePlusOfficial = 0xf2B3,
        //GooglePlusSquare, 0xf0D4
        GooglePlusSquare = 0xf0D4,
        //4.2, GoogleWallet, 0xf1Ee
        GoogleWallet = 0xf1Ee,
        //4.1, GraduationCap, 0xf19D
        GraduationCap = 0xf19D,
        //Gratipay, 0xf184
        Gratipay = 0xf184,
        //4.7, Grav, 0xf2D6
        Grav = 0xf2D6,
        //Group, 0xf0C0
        Group = 0xf0C0,
        //HSquare, 0xf0Fd
        HSquare = 0xf0Fd,
        //4.1, HackerNews, 0xf1D4
        HackerNews = 0xf1D4,
        //4.4, HandGrabO, 0xf255
        HandGrabO = 0xf255,
        //4.4, HandLizardO, 0xf258
        HandLizardO = 0xf258,
        //HandODown, 0xf0A7
        HandODown = 0xf0A7,
        //HandOLeft, 0xf0A5
        HandOLeft = 0xf0A5,
        //HandORight, 0xf0A4
        HandORight = 0xf0A4,
        //HandOUp, 0xf0A6
        HandOUp = 0xf0A6,
        //4.4, HandPaperO, 0xf256
        HandPaperO = 0xf256,
        //4.4, HandPeaceO, 0xf25B
        HandPeaceO = 0xf25B,
        //4.4, HandPointerO, 0xf25A
        HandPointerO = 0xf25A,
        //4.4, HandRockO, 0xf255
        HandRockO = 0xf255,
        //4.4, HandScissorsO, 0xf257
        HandScissorsO = 0xf257,
        //4.4, HandSpockO, 0xf259
        HandSpockO = 0xf259,
        //4.4, HandStopO, 0xf256
        HandStopO = 0xf256,
        //4.7, HandshakeO, 0xf2B5
        HandshakeO = 0xf2B5,
        //4.6, HardOfHearing, 0xf2A4
        HardOfHearing = 0xf2A4,
        //4.5, Hashtag, 0xf292
        Hashtag = 0xf292,
        //HddO, 0xf0A0
        HddO = 0xf0A0,
        //4.1, Header, 0xf1Dc
        Header = 0xf1Dc,
        //Headphones, 0xf025
        Headphones = 0xf025,
        //Heart, 0xf004
        Heart = 0xf004,
        //HeartO, 0xf08A
        HeartO = 0xf08A,
        //4.3, Heartbeat, 0xf21E
        Heartbeat = 0xf21E,
        //4.1, History, 0xf1Da
        History = 0xf1Da,
        //Home, 0xf015
        Home = 0xf015,
        //HospitalO, 0xf0F8
        HospitalO = 0xf0F8,
        //4.3, Hotel, 0xf236
        Hotel = 0xf236,
        //4.4, Hourglass, 0xf254
        Hourglass = 0xf254,
        //4.4, Hourglass1, 0xf251
        Hourglass1 = 0xf251,
        //4.4, Hourglass2, 0xf252
        Hourglass2 = 0xf252,
        //4.4, Hourglass3, 0xf253
        Hourglass3 = 0xf253,
        //4.4, HourglassEnd, 0xf253
        HourglassEnd = 0xf253,
        //4.4, HourglassHalf, 0xf252
        HourglassHalf = 0xf252,
        //4.4, HourglassO, 0xf250
        HourglassO = 0xf250,
        //4.4, HourglassStart, 0xf251
        HourglassStart = 0xf251,
        //4.4, Houzz, 0xf27C
        Houzz = 0xf27C,
        //Html5, 0xf13B
        Html5 = 0xf13B,
        //4.4, ICursor, 0xf246
        ICursor = 0xf246,
        //4.7, IdBadge, 0xf2C1
        IdBadge = 0xf2C1,
        //4.7, IdCard, 0xf2C2
        IdCard = 0xf2C2,
        //4.7, IdCardO, 0xf2C3
        IdCardO = 0xf2C3,
        //4.2, Ils, 0xf20B
        Ils = 0xf20B,
        //Image, 0xf03E
        Image = 0xf03E,
        //4.7, Imdb, 0xf2D8
        Imdb = 0xf2D8,
        //Inbox, 0xf01C
        Inbox = 0xf01C,
        //Indent, 0xf03C
        Indent = 0xf03C,
        //4.4, Industry, 0xf275
        Industry = 0xf275,
        //Info, 0xf129
        Info = 0xf129,
        //InfoCircle, 0xf05A
        InfoCircle = 0xf05A,
        //Inr, 0xf156
        Inr = 0xf156,
        //4.6, Instagram, 0xf16D
        Instagram = 0xf16D,
        //4.1, Institution, 0xf19C
        Institution = 0xf19C,
        //4.4, InternetExplorer, 0xf26B
        InternetExplorer = 0xf26B,
        //4.3, Intersex, 0xf224
        Intersex = 0xf224,
        //4.2, Ioxhost, 0xf208
        Ioxhost = 0xf208,
        //Italic, 0xf033
        Italic = 0xf033,
        //4.1, Joomla, 0xf1Aa
        Joomla = 0xf1Aa,
        //Jpy, 0xf157
        Jpy = 0xf157,
        //4.1, Jsfiddle, 0xf1Cc
        Jsfiddle = 0xf1Cc,
        //Key, 0xf084
        Key = 0xf084,
        //KeyboardO, 0xf11C
        KeyboardO = 0xf11C,
        //Krw, 0xf159
        Krw = 0xf159,
        //4.1, Language, 0xf1Ab
        Language = 0xf1Ab,
        //Laptop, 0xf109
        Laptop = 0xf109,
        //4.2, Lastfm, 0xf202
        Lastfm = 0xf202,
        //4.2, LastfmSquare, 0xf203
        LastfmSquare = 0xf203,
        //Leaf, 0xf06C
        Leaf = 0xf06C,
        //4.3, Leanpub, 0xf212
        Leanpub = 0xf212,
        //Legal, 0xf0E3
        Legal = 0xf0E3,
        //LemonO, 0xf094
        LemonO = 0xf094,
        //LevelDown, 0xf149
        LevelDown = 0xf149,
        //LevelUp, 0xf148
        LevelUp = 0xf148,
        //4.1, LifeBouy, 0xf1Cd
        LifeBouy = 0xf1Cd,
        //4.1, LifeBuoy, 0xf1Cd
        LifeBuoy = 0xf1Cd,
        //4.1, LifeRing, 0xf1Cd
        LifeRing = 0xf1Cd,
        //4.1, LifeSaver, 0xf1Cd
        LifeSaver = 0xf1Cd,
        //LightbulbO, 0xf0Eb
        LightbulbO = 0xf0Eb,
        //4.2, LineChart, 0xf201
        LineChart = 0xf201,
        //Link, 0xf0C1
        Link = 0xf0C1,
        //Linkedin, 0xf0E1
        Linkedin = 0xf0E1,
        //LinkedinSquare, 0xf08C
        LinkedinSquare = 0xf08C,
        //4.7, Linode, 0xf2B8
        Linode = 0xf2B8,
        //Linux, 0xf17C
        Linux = 0xf17C,
        //List, 0xf03A
        List = 0xf03A,
        //ListAlt, 0xf022
        ListAlt = 0xf022,
        //ListOl, 0xf0Cb
        ListOl = 0xf0Cb,
        //ListUl, 0xf0Ca
        ListUl = 0xf0Ca,
        //LocationArrow, 0xf124
        LocationArrow = 0xf124,
        //Lock, 0xf023
        Lock = 0xf023,
        //LongArrowDown, 0xf175
        LongArrowDown = 0xf175,
        //LongArrowLeft, 0xf177
        LongArrowLeft = 0xf177,
        //LongArrowRight, 0xf178
        LongArrowRight = 0xf178,
        //LongArrowUp, 0xf176
        LongArrowUp = 0xf176,
        //4.6, LowVision, 0xf2A8
        LowVision = 0xf2A8,
        //Magic, 0xf0D0
        Magic = 0xf0D0,
        //Magnet, 0xf076
        Magnet = 0xf076,
        //MailForward, 0xf064
        MailForward = 0xf064,
        //MailReply, 0xf112
        MailReply = 0xf112,
        //MailReplyAll, 0xf122
        MailReplyAll = 0xf122,
        //Male, 0xf183
        Male = 0xf183,
        //4.4, Map, 0xf279
        Map = 0xf279,
        //MapMarker, 0xf041
        MapMarker = 0xf041,
        //4.4, MapO, 0xf278
        MapO = 0xf278,
        //4.4, MapPin, 0xf276
        MapPin = 0xf276,
        //4.4, MapSigns, 0xf277
        MapSigns = 0xf277,
        //4.3, Mars, 0xf222
        Mars = 0xf222,
        //4.3, MarsDouble, 0xf227
        MarsDouble = 0xf227,
        //4.3, MarsStroke, 0xf229
        MarsStroke = 0xf229,
        //4.3, MarsStrokeH, 0xf22B
        MarsStrokeH = 0xf22B,
        //4.3, MarsStrokeV, 0xf22A
        MarsStrokeV = 0xf22A,
        //Maxcdn, 0xf136
        Maxcdn = 0xf136,
        //4.2, Meanpath, 0xf20C
        Meanpath = 0xf20C,
        //4.3, Medium, 0xf23A
        Medium = 0xf23A,
        //Medkit, 0xf0Fa
        Medkit = 0xf0Fa,
        //4.7, Meetup, 0xf2E0
        Meetup = 0xf2E0,
        //MehO, 0xf11A
        MehO = 0xf11A,
        //4.3, Mercury, 0xf223
        Mercury = 0xf223,
        //4.7, Microchip, 0xf2Db
        Microchip = 0xf2Db,
        //Microphone, 0xf130
        Microphone = 0xf130,
        //MicrophoneSlash, 0xf131
        MicrophoneSlash = 0xf131,
        //Minus, 0xf068
        Minus = 0xf068,
        //MinusCircle, 0xf056
        MinusCircle = 0xf056,
        //MinusSquare, 0xf146
        MinusSquare = 0xf146,
        //MinusSquareO, 0xf147
        MinusSquareO = 0xf147,
        //4.5, Mixcloud, 0xf289
        Mixcloud = 0xf289,
        //Mobile, 0xf10B
        Mobile = 0xf10B,
        //MobilePhone, 0xf10B
        MobilePhone = 0xf10B,
        //4.5, Modx, 0xf285
        Modx = 0xf285,
        //Money, 0xf0D6
        Money = 0xf0D6,
        //MoonO, 0xf186
        MoonO = 0xf186,
        //4.1, MortarBoard, 0xf19D
        MortarBoard = 0xf19D,
        //4.3, Motorcycle, 0xf21C
        Motorcycle = 0xf21C,
        //4.4, MousePointer, 0xf245
        MousePointer = 0xf245,
        //Music, 0xf001
        Music = 0xf001,
        //Navicon, 0xf0C9
        Navicon = 0xf0C9,
        //4.3, Neuter, 0xf22C
        Neuter = 0xf22C,
        //4.2, NewspaperO, 0xf1Ea
        NewspaperO = 0xf1Ea,
        //4.4, ObjectGroup, 0xf247
        ObjectGroup = 0xf247,
        //4.4, ObjectUngroup, 0xf248
        ObjectUngroup = 0xf248,
        //4.4, Odnoklassniki, 0xf263
        Odnoklassniki = 0xf263,
        //4.4, OdnoklassnikiSquare, 0xf264
        OdnoklassnikiSquare = 0xf264,
        //4.4, Opencart, 0xf23D
        Opencart = 0xf23D,
        //4.1, Openid, 0xf19B
        Openid = 0xf19B,
        //4.4, Opera, 0xf26A
        Opera = 0xf26A,
        //4.4, OptinMonster, 0xf23C
        OptinMonster = 0xf23C,
        //Outdent, 0xf03B
        Outdent = 0xf03B,
        //4.0, Pagelines, 0xf18C
        Pagelines = 0xf18C,
        //4.2, PaintBrush, 0xf1Fc
        PaintBrush = 0xf1Fc,
        //4.1, PaperPlane, 0xf1D8
        PaperPlane = 0xf1D8,
        //4.1, PaperPlaneO, 0xf1D9
        PaperPlaneO = 0xf1D9,
        //Paperclip, 0xf0C6
        Paperclip = 0xf0C6,
        //4.1, Paragraph, 0xf1Dd
        Paragraph = 0xf1Dd,
        //Paste, 0xf0Ea
        Paste = 0xf0Ea,
        //Pause, 0xf04C
        Pause = 0xf04C,
        //4.5, PauseCircle, 0xf28B
        PauseCircle = 0xf28B,
        //4.5, PauseCircleO, 0xf28C
        PauseCircleO = 0xf28C,
        //4.1, Paw, 0xf1B0
        Paw = 0xf1B0,
        //4.2, Paypal, 0xf1Ed
        Paypal = 0xf1Ed,
        //Pencil, 0xf040
        Pencil = 0xf040,
        //PencilSquare, 0xf14B
        PencilSquare = 0xf14B,
        //PencilSquareO, 0xf044
        PencilSquareO = 0xf044,
        //4.5, Percent, 0xf295
        Percent = 0xf295,
        //Phone, 0xf095
        Phone = 0xf095,
        //PhoneSquare, 0xf098
        PhoneSquare = 0xf098,
        //Photo, 0xf03E
        Photo = 0xf03E,
        //PictureO, 0xf03E
        PictureO = 0xf03E,
        //4.2, PieChart, 0xf200
        PieChart = 0xf200,
        //4.6, PiedPiper, 0xf2Ae
        PiedPiper = 0xf2Ae,
        //4.1, PiedPiperAlt, 0xf1A8
        PiedPiperAlt = 0xf1A8,
        //4.1, PiedPiperPp, 0xf1A7
        PiedPiperPp = 0xf1A7,
        //Pinterest, 0xf0D2
        Pinterest = 0xf0D2,
        //4.3, PinterestP, 0xf231
        PinterestP = 0xf231,
        //PinterestSquare, 0xf0D3
        PinterestSquare = 0xf0D3,
        //Plane, 0xf072
        Plane = 0xf072,
        //Play, 0xf04B
        Play = 0xf04B,
        //PlayCircle, 0xf144
        PlayCircle = 0xf144,
        //PlayCircleO, 0xf01D
        PlayCircleO = 0xf01D,
        //4.2, Plug, 0xf1E6
        Plug = 0xf1E6,
        //Plus, 0xf067
        Plus = 0xf067,
        //PlusCircle, 0xf055
        PlusCircle = 0xf055,
        //PlusSquare, 0xf0Fe
        PlusSquare = 0xf0Fe,
        //4.0, PlusSquareO, 0xf196
        PlusSquareO = 0xf196,
        //4.7, Podcast, 0xf2Ce
        Podcast = 0xf2Ce,
        //PowerOff, 0xf011
        PowerOff = 0xf011,
        //Print, 0xf02F
        Print = 0xf02F,
        //4.5, ProductHunt, 0xf288
        ProductHunt = 0xf288,
        //PuzzlePiece, 0xf12E
        PuzzlePiece = 0xf12E,
        //4.1, Qq, 0xf1D6
        Qq = 0xf1D6,
        //Qrcode, 0xf029
        Qrcode = 0xf029,
        //Question, 0xf128
        Question = 0xf128,
        //QuestionCircle, 0xf059
        QuestionCircle = 0xf059,
        //4.6, QuestionCircleO, 0xf29C
        QuestionCircleO = 0xf29C,
        //4.7, Quora, 0xf2C4
        Quora = 0xf2C4,
        //QuoteLeft, 0xf10D
        QuoteLeft = 0xf10D,
        //QuoteRight, 0xf10E
        QuoteRight = 0xf10E,
        //4.1, Ra, 0xf1D0
        Ra = 0xf1D0,
        //Random, 0xf074
        Random = 0xf074,
        //4.7, Ravelry, 0xf2D9
        Ravelry = 0xf2D9,
        //4.1, Rebel, 0xf1D0
        Rebel = 0xf1D0,
        //4.1, Recycle, 0xf1B8
        Recycle = 0xf1B8,
        //4.1, Reddit, 0xf1A1
        Reddit = 0xf1A1,
        //4.5, RedditAlien, 0xf281
        RedditAlien = 0xf281,
        //4.1, RedditSquare, 0xf1A2
        RedditSquare = 0xf1A2,
        //Refresh, 0xf021
        Refresh = 0xf021,
        //4.4, Registered, 0xf25D
        Registered = 0xf25D,
        //Remove, 0xf00D
        Remove = 0xf00D,
        //Renren, 0xf18B
        Renren = 0xf18B,
        //Reorder, 0xf0C9
        Reorder = 0xf0C9,
        //Repeat, 0xf01E
        Repeat = 0xf01E,
        //Reply, 0xf112
        Reply = 0xf112,
        //ReplyAll, 0xf122
        ReplyAll = 0xf122,
        //4.1, Resistance, 0xf1D0
        Resistance = 0xf1D0,
        //Retweet, 0xf079
        Retweet = 0xf079,
        //Rmb, 0xf157
        Rmb = 0xf157,
        //Road, 0xf018
        Road = 0xf018,
        //Rocket, 0xf135
        Rocket = 0xf135,
        //RotateLeft, 0xf0E2
        RotateLeft = 0xf0E2,
        //RotateRight, 0xf01E
        RotateRight = 0xf01E,
        //4.0, Rouble, 0xf158
        Rouble = 0xf158,
        //Rss, 0xf09E
        Rss = 0xf09E,
        //RssSquare, 0xf143
        RssSquare = 0xf143,
        //4.0, Rub, 0xf158
        Rub = 0xf158,
        //4.0, Ruble, 0xf158
        Ruble = 0xf158,
        //Rupee, 0xf156
        Rupee = 0xf156,
        //4.7, S15, 0xf2Cd
        S15 = 0xf2Cd,
        //4.4, Safari, 0xf267
        Safari = 0xf267,
        //Save, 0xf0C7
        Save = 0xf0C7,
        //Scissors, 0xf0C4
        Scissors = 0xf0C4,
        //4.5, Scribd, 0xf28A
        Scribd = 0xf28A,
        //Search, 0xf002
        Search = 0xf002,
        //SearchMinus, 0xf010
        SearchMinus = 0xf010,
        //SearchPlus, 0xf00E
        SearchPlus = 0xf00E,
        //4.3, Sellsy, 0xf213
        Sellsy = 0xf213,
        //4.1, Send, 0xf1D8
        Send = 0xf1D8,
        //4.1, SendO, 0xf1D9
        SendO = 0xf1D9,
        //4.3, Server, 0xf233
        Server = 0xf233,
        //Share, 0xf064
        Share = 0xf064,
        //4.1, ShareAlt, 0xf1E0
        ShareAlt = 0xf1E0,
        //4.1, ShareAltSquare, 0xf1E1
        ShareAltSquare = 0xf1E1,
        //ShareSquare, 0xf14D
        ShareSquare = 0xf14D,
        //ShareSquareO, 0xf045
        ShareSquareO = 0xf045,
        //4.2, Shekel, 0xf20B
        Shekel = 0xf20B,
        //4.2, Sheqel, 0xf20B
        Sheqel = 0xf20B,
        //Shield, 0xf132
        Shield = 0xf132,
        //4.3, Ship, 0xf21A
        Ship = 0xf21A,
        //4.3, Shirtsinbulk, 0xf214
        Shirtsinbulk = 0xf214,
        //4.5, ShoppingBag, 0xf290
        ShoppingBag = 0xf290,
        //4.5, ShoppingBasket, 0xf291
        ShoppingBasket = 0xf291,
        //ShoppingCart, 0xf07A
        ShoppingCart = 0xf07A,
        //4.7, Shower, 0xf2Cc
        Shower = 0xf2Cc,
        //SignIn, 0xf090
        SignIn = 0xf090,
        //4.6, SignLanguage, 0xf2A7
        SignLanguage = 0xf2A7,
        //SignOut, 0xf08B
        SignOut = 0xf08B,
        //Signal, 0xf012
        Signal = 0xf012,
        //4.6, Signing, 0xf2A7
        Signing = 0xf2A7,
        //4.3, Simplybuilt, 0xf215
        Simplybuilt = 0xf215,
        //Sitemap, 0xf0E8
        Sitemap = 0xf0E8,
        //4.3, Skyatlas, 0xf216
        Skyatlas = 0xf216,
        //Skype, 0xf17E
        Skype = 0xf17E,
        //4.1, Slack, 0xf198
        Slack = 0xf198,
        //4.1, Sliders, 0xf1De
        Sliders = 0xf1De,
        //4.2, Slideshare, 0xf1E7
        Slideshare = 0xf1E7,
        //SmileO, 0xf118
        SmileO = 0xf118,
        //4.6, Snapchat, 0xf2Ab
        Snapchat = 0xf2Ab,
        //4.6, SnapchatGhost, 0xf2Ac
        SnapchatGhost = 0xf2Ac,
        //4.6, SnapchatSquare, 0xf2Ad
        SnapchatSquare = 0xf2Ad,
        //4.7, SnowflakeO, 0xf2Dc
        SnowflakeO = 0xf2Dc,
        //4.2, SoccerBallO, 0xf1E3
        SoccerBallO = 0xf1E3,
        //Sort, 0xf0Dc
        Sort = 0xf0Dc,
        //SortAlphaAsc, 0xf15D
        SortAlphaAsc = 0xf15D,
        //SortAlphaDesc, 0xf15E
        SortAlphaDesc = 0xf15E,
        //SortAmountAsc, 0xf160
        SortAmountAsc = 0xf160,
        //SortAmountDesc, 0xf161
        SortAmountDesc = 0xf161,
        //SortAsc, 0xf0De
        SortAsc = 0xf0De,
        //SortDesc, 0xf0Dd
        SortDesc = 0xf0Dd,
        //SortDown, 0xf0Dd
        SortDown = 0xf0Dd,
        //SortNumericAsc, 0xf162
        SortNumericAsc = 0xf162,
        //SortNumericDesc, 0xf163
        SortNumericDesc = 0xf163,
        //SortUp, 0xf0De
        SortUp = 0xf0De,
        //4.1, Soundcloud, 0xf1Be
        Soundcloud = 0xf1Be,
        //4.1, SpaceShuttle, 0xf197
        SpaceShuttle = 0xf197,
        //Spinner, 0xf110
        Spinner = 0xf110,
        //4.1, Spoon, 0xf1B1
        Spoon = 0xf1B1,
        //4.1, Spotify, 0xf1Bc
        Spotify = 0xf1Bc,
        //Square, 0xf0C8
        Square = 0xf0C8,
        //SquareO, 0xf096
        SquareO = 0xf096,
        //4.0, StackExchange, 0xf18D
        StackExchange = 0xf18D,
        //StackOverflow, 0xf16C
        StackOverflow = 0xf16C,
        //Star, 0xf005
        Star = 0xf005,
        //StarHalf, 0xf089
        StarHalf = 0xf089,
        //StarHalfEmpty, 0xf123
        StarHalfEmpty = 0xf123,
        //StarHalfFull, 0xf123
        StarHalfFull = 0xf123,
        //StarHalfO, 0xf123
        StarHalfO = 0xf123,
        //StarO, 0xf006
        StarO = 0xf006,
        //4.1, Steam, 0xf1B6
        Steam = 0xf1B6,
        //4.1, SteamSquare, 0xf1B7
        SteamSquare = 0xf1B7,
        //StepBackward, 0xf048
        StepBackward = 0xf048,
        //StepForward, 0xf051
        StepForward = 0xf051,
        //Stethoscope, 0xf0F1
        Stethoscope = 0xf0F1,
        //4.4, StickyNote, 0xf249
        StickyNote = 0xf249,
        //4.4, StickyNoteO, 0xf24A
        StickyNoteO = 0xf24A,
        //Stop, 0xf04D
        Stop = 0xf04D,
        //4.5, StopCircle, 0xf28D
        StopCircle = 0xf28D,
        //4.5, StopCircleO, 0xf28E
        StopCircleO = 0xf28E,
        //4.3, StreetView, 0xf21D
        StreetView = 0xf21D,
        //Strikethrough, 0xf0Cc
        Strikethrough = 0xf0Cc,
        //4.1, Stumbleupon, 0xf1A4
        Stumbleupon = 0xf1A4,
        //4.1, StumbleuponCircle, 0xf1A3
        StumbleuponCircle = 0xf1A3,
        //Subscript, 0xf12C
        Subscript = 0xf12C,
        //4.3, Subway, 0xf239
        Subway = 0xf239,
        //Suitcase, 0xf0F2
        Suitcase = 0xf0F2,
        //SunO, 0xf185
        SunO = 0xf185,
        //4.7, Superpowers, 0xf2Dd
        Superpowers = 0xf2Dd,
        //Superscript, 0xf12B
        Superscript = 0xf12B,
        //4.1, Support, 0xf1Cd
        Support = 0xf1Cd,
        //Table, 0xf0Ce
        Table = 0xf0Ce,
        //Tablet, 0xf10A
        Tablet = 0xf10A,
        //Tachometer, 0xf0E4
        Tachometer = 0xf0E4,
        //Tag, 0xf02B
        Tag = 0xf02B,
        //Tags, 0xf02C
        Tags = 0xf02C,
        //Tasks, 0xf0Ae
        Tasks = 0xf0Ae,
        //4.1, Taxi, 0xf1Ba
        Taxi = 0xf1Ba,
        //4.7, Telegram, 0xf2C6
        Telegram = 0xf2C6,
        //4.4, Television, 0xf26C
        Television = 0xf26C,
        //4.1, TencentWeibo, 0xf1D5
        TencentWeibo = 0xf1D5,
        //Terminal, 0xf120
        Terminal = 0xf120,
        //TextHeight, 0xf034
        TextHeight = 0xf034,
        //TextWidth, 0xf035
        TextWidth = 0xf035,
        //Th, 0xf00A
        Th = 0xf00A,
        //ThLarge, 0xf009
        ThLarge = 0xf009,
        //ThList, 0xf00B
        ThList = 0xf00B,
        //4.6, Themeisle, 0xf2B2
        Themeisle = 0xf2B2,
        //4.7, Thermometer, 0xf2C7
        Thermometer = 0xf2C7,
        //4.7, Thermometer0, 0xf2Cb
        Thermometer0 = 0xf2Cb,
        //4.7, Thermometer1, 0xf2Ca
        Thermometer1 = 0xf2Ca,
        //4.7, Thermometer2, 0xf2C9
        Thermometer2 = 0xf2C9,
        //4.7, Thermometer3, 0xf2C8
        Thermometer3 = 0xf2C8,
        //4.7, Thermometer4, 0xf2C7
        Thermometer4 = 0xf2C7,
        //4.7, ThermometerEmpty, 0xf2Cb
        ThermometerEmpty = 0xf2Cb,
        //4.7, ThermometerFull, 0xf2C7
        ThermometerFull = 0xf2C7,
        //4.7, ThermometerHalf, 0xf2C9
        ThermometerHalf = 0xf2C9,
        //4.7, ThermometerQuarter, 0xf2Ca
        ThermometerQuarter = 0xf2Ca,
        //4.7, ThermometerThreeQuarters, 0xf2C8
        ThermometerThreeQuarters = 0xf2C8,
        //ThumbTack, 0xf08D
        ThumbTack = 0xf08D,
        //ThumbsDown, 0xf165
        ThumbsDown = 0xf165,
        //ThumbsODown, 0xf088
        ThumbsODown = 0xf088,
        //ThumbsOUp, 0xf087
        ThumbsOUp = 0xf087,
        //ThumbsUp, 0xf164
        ThumbsUp = 0xf164,
        //Ticket, 0xf145
        Ticket = 0xf145,
        //Times, 0xf00D
        Times = 0xf00D,
        //TimesCircle, 0xf057
        TimesCircle = 0xf057,
        //TimesCircleO, 0xf05C
        TimesCircleO = 0xf05C,
        //4.7, TimesRectangle, 0xf2D3
        TimesRectangle = 0xf2D3,
        //4.7, TimesRectangleO, 0xf2D4
        TimesRectangleO = 0xf2D4,
        //Tint, 0xf043
        Tint = 0xf043,
        //ToggleDown, 0xf150
        ToggleDown = 0xf150,
        //4.0, ToggleLeft, 0xf191
        ToggleLeft = 0xf191,
        //4.2, ToggleOff, 0xf204
        ToggleOff = 0xf204,
        //4.2, ToggleOn, 0xf205
        ToggleOn = 0xf205,
        //ToggleRight, 0xf152
        ToggleRight = 0xf152,
        //ToggleUp, 0xf151
        ToggleUp = 0xf151,
        //4.4, Trademark, 0xf25C
        Trademark = 0xf25C,
        //4.3, Train, 0xf238
        Train = 0xf238,
        //4.3, Transgender, 0xf224
        Transgender = 0xf224,
        //4.3, TransgenderAlt, 0xf225
        TransgenderAlt = 0xf225,
        //4.2, Trash, 0xf1F8
        Trash = 0xf1F8,
        //TrashO, 0xf014
        TrashO = 0xf014,
        //4.1, Tree, 0xf1Bb
        Tree = 0xf1Bb,
        //Trello, 0xf181
        Trello = 0xf181,
        //4.4, Tripadvisor, 0xf262
        Tripadvisor = 0xf262,
        //Trophy, 0xf091
        Trophy = 0xf091,
        //Truck, 0xf0D1
        Truck = 0xf0D1,
        //4.0, Try, 0xf195
        Try = 0xf195,
        //4.2, Tty, 0xf1E4
        Tty = 0xf1E4,
        //Tumblr, 0xf173
        Tumblr = 0xf173,
        //TumblrSquare, 0xf174
        TumblrSquare = 0xf174,
        //4.0, TurkishLira, 0xf195
        TurkishLira = 0xf195,
        //4.4, Tv, 0xf26C
        Tv = 0xf26C,
        //4.2, Twitch, 0xf1E8
        Twitch = 0xf1E8,
        //Twitter, 0xf099
        Twitter = 0xf099,
        //TwitterSquare, 0xf081
        TwitterSquare = 0xf081,
        //Umbrella, 0xf0E9
        Umbrella = 0xf0E9,
        //Underline, 0xf0Cd
        Underline = 0xf0Cd,
        //Undo, 0xf0E2
        Undo = 0xf0E2,
        //4.6, UniversalAccess, 0xf29A
        UniversalAccess = 0xf29A,
        //4.1, University, 0xf19C
        University = 0xf19C,
        //Unlink, 0xf127
        Unlink = 0xf127,
        //Unlock, 0xf09C
        Unlock = 0xf09C,
        //UnlockAlt, 0xf13E
        UnlockAlt = 0xf13E,
        //Unsorted, 0xf0Dc
        Unsorted = 0xf0Dc,
        //Upload, 0xf093
        Upload = 0xf093,
        //4.5, Usb, 0xf287
        Usb = 0xf287,
        //Usd, 0xf155
        Usd = 0xf155,
        //User, 0xf007
        User = 0xf007,
        //4.7, UserCircle, 0xf2Bd
        UserCircle = 0xf2Bd,
        //4.7, UserCircleO, 0xf2Be
        UserCircleO = 0xf2Be,
        //UserMd, 0xf0F0
        UserMd = 0xf0F0,
        //4.7, UserO, 0xf2C0
        UserO = 0xf2C0,
        //4.3, UserPlus, 0xf234
        UserPlus = 0xf234,
        //4.3, UserSecret, 0xf21B
        UserSecret = 0xf21B,
        //4.3, UserTimes, 0xf235
        UserTimes = 0xf235,
        //Users, 0xf0C0
        Users = 0xf0C0,
        //4.7, Vcard, 0xf2Bb
        Vcard = 0xf2Bb,
        //4.7, VcardO, 0xf2Bc
        VcardO = 0xf2Bc,
        //4.3, Venus, 0xf221
        Venus = 0xf221,
        //4.3, VenusDouble, 0xf226
        VenusDouble = 0xf226,
        //4.3, VenusMars, 0xf228
        VenusMars = 0xf228,
        //4.3, Viacoin, 0xf237
        Viacoin = 0xf237,
        //4.6, Viadeo, 0xf2A9
        Viadeo = 0xf2A9,
        //4.6, ViadeoSquare, 0xf2Aa
        ViadeoSquare = 0xf2Aa,
        //VideoCamera, 0xf03D
        VideoCamera = 0xf03D,
        //4.4, Vimeo, 0xf27D
        Vimeo = 0xf27D,
        //4.0, VimeoSquare, 0xf194
        VimeoSquare = 0xf194,
        //4.1, Vine, 0xf1Ca
        Vine = 0xf1Ca,
        //Vk, 0xf189
        Vk = 0xf189,
        //4.6, VolumeControlPhone, 0xf2A0
        VolumeControlPhone = 0xf2A0,
        //VolumeDown, 0xf027
        VolumeDown = 0xf027,
        //VolumeOff, 0xf026
        VolumeOff = 0xf026,
        //VolumeUp, 0xf028
        VolumeUp = 0xf028,
        //Warning, 0xf071
        Warning = 0xf071,
        //4.1, Wechat, 0xf1D7
        Wechat = 0xf1D7,
        //Weibo, 0xf18A
        Weibo = 0xf18A,
        //4.1, Weixin, 0xf1D7
        Weixin = 0xf1D7,
        //4.3, Whatsapp, 0xf232
        Whatsapp = 0xf232,
        //4.0, Wheelchair, 0xf193
        Wheelchair = 0xf193,
        //4.6, WheelchairAlt, 0xf29B
        WheelchairAlt = 0xf29B,
        //4.2, Wifi, 0xf1Eb
        Wifi = 0xf1Eb,
        //4.4, WikipediaW, 0xf266
        WikipediaW = 0xf266,
        //4.7, WindowClose, 0xf2D3
        WindowClose = 0xf2D3,
        //4.7, WindowCloseO, 0xf2D4
        WindowCloseO = 0xf2D4,
        //4.7, WindowMaximize, 0xf2D0
        WindowMaximize = 0xf2D0,
        //4.7, WindowMinimize, 0xf2D1
        WindowMinimize = 0xf2D1,
        //4.7, WindowRestore, 0xf2D2
        WindowRestore = 0xf2D2,
        //Windows, 0xf17A
        Windows = 0xf17A,
        //Won, 0xf159
        Won = 0xf159,
        //4.1, Wordpress, 0xf19A
        Wordpress = 0xf19A,
        //4.6, Wpbeginner, 0xf297
        Wpbeginner = 0xf297,
        //4.7, Wpexplorer, 0xf2De
        Wpexplorer = 0xf2De,
        //4.6, Wpforms, 0xf298
        Wpforms = 0xf298,
        //Wrench, 0xf0Ad
        Wrench = 0xf0Ad,
        //Xing, 0xf168
        Xing = 0xf168,
        //XingSquare, 0xf169
        XingSquare = 0xf169,
        //4.4, YCombinator, 0xf23B
        YCombinator = 0xf23B,
        //4.1, YCombinatorSquare, 0xf1D4
        YCombinatorSquare = 0xf1D4,
        //4.1, Yahoo, 0xf19E
        Yahoo = 0xf19E,
        //4.4, Yc, 0xf23B
        Yc = 0xf23B,
        //4.1, YcSquare, 0xf1D4
        YcSquare = 0xf1D4,
        //4.2, Yelp, 0xf1E9
        Yelp = 0xf1E9,
        //Yen, 0xf157
        Yen = 0xf157,
        //4.6, Yoast, 0xf2B1
        Yoast = 0xf2B1,
        //Youtube, 0xf167
        Youtube = 0xf167,
        //YoutubePlay, 0xf16A
        YoutubePlay = 0xf16A,
        //YoutubeSquare, 0xf166
        YoutubeSquare = 0xf166
    }
    #endregion
}

public static class RbControls_FontAwesomeExtensions
{
    public static Bitmap StackWith(this Bitmap backgroundImage, RbControls_FontAwesome.Properties foregroundImage)
    {
        var bitmap = backgroundImage;
        Graphics g = Graphics.FromImage(backgroundImage);
        g.DrawImage(RbControls_FontAwesome.Instance.GetImage(foregroundImage), new Point(0, 0));
        return bitmap;
    }

    public static Bitmap StackWith(this Bitmap backgroundImage, Bitmap foregroundImage)
    {
        var bitmap = backgroundImage;
        Graphics g = Graphics.FromImage(backgroundImage);
        g.DrawImage(foregroundImage, new Point(0, 0));
        return bitmap;
    }

    public static Bitmap AsImage(this RbControls_FontAwesome.Type type, RbControls_FontAwesome.Properties fontProperties = null)
    {
        if (fontProperties == null)
        {
            fontProperties = RbControls_FontAwesome.Properties.Get(type);
        }
        else
        {
            fontProperties.Type = type;
        }
        return fontProperties.AsImage();
    }

    public static Bitmap AsImage(this RbControls_FontAwesome.Properties fontProperties)
    {
        return RbControls_FontAwesome.Instance.GetImage(fontProperties);
    }

    public static Icon AsIcon(this RbControls_FontAwesome.Type type, RbControls_FontAwesome.Properties fontProperties = null)
    {
        if (fontProperties == null)
        {
            fontProperties = RbControls_FontAwesome.Properties.Get(type);
        }
        else
        {
            fontProperties.Type = type;
        }
        return fontProperties.AsIcon();
    }

    public static Icon AsIcon(this RbControls_FontAwesome.Properties fontProperties)
    {
        return RbControls_FontAwesome.Instance.GetIcon(fontProperties);
    }
}

