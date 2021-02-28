using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace NoTrashLeftBehind
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteSheet _spriteSheet;
        private SpriteSheet _trashSheet;
        private SpriteSheet _overviewSheet;

        Grid grid;
        Board board;
        Object overview;

        bool resetting = false;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            SpriteMapper mapper = new SpriteMapper();
            _spriteSheet = mapper.ReadFile(Config.SHEET_CONFIG_FILE_NAME, Content);
            _trashSheet = mapper.ReadFile(Config.TRASH_SHEET_CONFIG_FILE_NAME, Content);

            _graphics.PreferredBackBufferWidth = Config.VIEWPORT_WIDTH;
            _graphics.PreferredBackBufferHeight = Config.VIEWPORT_HEIGHT;
            _graphics.ApplyChanges();

            //RectangleBody collision = new RectangleBody(new Vector2(100, 100), new Vector2(200, 200));
            //testObj = new Object(this, collision);

            //Config.GRID_INFO gridInfo = new Config.GRID_INFO(50, 25, 0f, 0f, 1.0f, 1.0f);
            //testGrid = new Grid(gridInfo);

            grid = new Grid(Config.GAME_GRID);
            RectangleBody size = new RectangleBody(Config.GAME_GRID.Rect);
            board = new Board(this, size);
            board.Initialize(Config.GAME_GRID.TilesWide, Config.GAME_GRID.TilesHigh, 16);

            _overviewSheet = mapper.ReadFile(Config.OVERVIEW_CONFIG_FILE_NAME, Content);
            RectangleBody fullSize = new RectangleBody(new Vector2(0, 0), new Vector2(Config.VIEWPORT_WIDTH, Config.VIEWPORT_HEIGHT));
            overview = new Object(this, fullSize);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _spriteSheet.LoadContent(Content);
            _trashSheet.LoadContent(Content);
            _overviewSheet.LoadContent(Content);

            Sprite selection = _spriteSheet.GetSprite("blank");
            selection.TextureColor = Config.SELECTION_COLOR * Config.SELECTION_OPACITY;
            board.LoadContent(_spriteSheet.GetSprite("tile"), _trashSheet, selection);

            grid.LoadContent(_spriteSheet.GetSprite("tile"));
            overview.LoadContent(_overviewSheet.GetSprite("overview"));
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (!resetting && Keyboard.GetState().IsKeyDown(Keys.R))
            {
                resetting = true;
                board.ScrambleBoard();
                board.ScrambleTrashTextures();
            }
            else if(resetting && Keyboard.GetState().IsKeyUp(Keys.R))
            {
                resetting = false;
            }

            board.Update(Mouse.GetState());

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            _spriteBatch.Begin(SpriteSortMode.BackToFront, null, SamplerState.PointClamp);
            overview.Draw(_spriteBatch);
            _spriteBatch.End();

            //grid.Draw(_spriteBatch);

            board.Draw(_spriteBatch);

            base.Draw(gameTime);
        }
    }
}
