using Microsoft.Xna.Framework;
using TBAR.UI.ScreenEffects.TimeSkip;

namespace TBAR.UI
{
    public class UIManager
    {
        internal void Initialize()
        {
            TimeSkipLayer = new TimeSkipLayer();
            StandAlbumLayer = new StandAlbumLayer();
            ResourceLayer = new ResourceLayer();
        }

        public void Update(GameTime gameTime)
        {
            TimeSkipLayer.State.Update(gameTime);

            StandAlbumLayer.Update(gameTime);

            ResourceLayer.Update(gameTime);
        }

        public TimeSkipLayer TimeSkipLayer { get; set; }

        public StandAlbumLayer StandAlbumLayer { get; set; }

        public ResourceLayer ResourceLayer { get; set; }

        public static void Unload()
        {
            _instance = null;
        }

        private static UIManager _instance;

        public static UIManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new UIManager();

                return _instance;
            }
        }

        private UIManager() { }
    }
}
