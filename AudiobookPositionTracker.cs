using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Timers;
using System.Windows.Forms;

namespace MusicBeePlugin
{
    public partial class Plugin
    {
        private readonly PluginInfo _about = new PluginInfo();
        private MusicBeeApiInterface _mbApiInterface;

        private static System.Timers.Timer _playingTimer;

        public PluginInfo Initialise(IntPtr apiInterfacePtr)
        {
            _playingTimer = new System.Timers.Timer(1000);
            _playingTimer.Elapsed += WhilePlaying;
            _playingTimer.AutoReset = true;

            _mbApiInterface = new MusicBeeApiInterface();
            _mbApiInterface.Initialise(apiInterfacePtr);

            _about.PluginInfoVersion = PluginInfoVersion;
            _about.Name = "Audiobook Position Tracker";
            _about.Description = "Remembers where you stopped listening in each audiobook.";
            _about.Author = "Shaun Simmons";
            _about.TargetApplication =
                ""; // current only applies to artwork, lyrics or instant messenger name that appears in the provider drop down selector or target Instant Messenger
            _about.Type = PluginType.General;
            _about.VersionMajor = 1; // your plugin version
            _about.VersionMinor = 0;
            _about.Revision = 1;
            _about.MinInterfaceVersion = MinInterfaceVersion;
            _about.MinApiRevision = MinApiRevision;
            _about.ReceiveNotifications = ReceiveNotificationFlags.PlayerEvents;
            _about.ConfigurationPanelHeight =
                0; // height in pixels that MusicBee should reserve in a panel for config settings. When set, a handle to an empty panel will be passed to the Configure function

            return _about;
        }

        public bool Configure(IntPtr panelHandle)
        {
            return false;
        }

        // called by MusicBee when the user clicks Apply or Save in the MusicBee Preferences screen.
        // its up to you to figure out whether anything has changed and needs updating
        public void SaveSettings()
        {
        }

        // MusicBee is closing the plugin (plugin is being disabled by user or MusicBee is shutting down)
        public void Close(PluginCloseReason reason)
        {
            _playingTimer.Stop();
        }

        // uninstall this plugin - clean up any persisted files
        public void Uninstall()
        {
        }

        private static string _currentTrackUrl;

        public string CurrentTrackUrl()
        {
            return _mbApiInterface.NowPlaying_GetFileProperty(FilePropertyType.Url);
        }

        public string GetPositionFilename()
        {
            return CurrentTrackUrl() + ".position.txt";
        }

        public void LoadSavedPosition()
        {
            try
            {
                var text = File.ReadAllText(GetPositionFilename());
                _mbApiInterface.Player_SetPosition(int.Parse(text));
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public void SavePosition()
        {
            try
            {
                var position = _mbApiInterface.Player_GetPosition();
                File.WriteAllText(CurrentTrackUrl() + ".position.txt", position.ToString());
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public void WhilePlaying(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            SavePosition();
        }

        // receive event notifications from MusicBee
        // you need to set about.ReceiveNotificationFlags = PlayerEvents to receive all notifications, and not just the startup event
        public void ReceiveNotification(string sourceFileUrl, NotificationType type)
        {
            switch (type)
            {
                case NotificationType.PlayStateChanged:
                    switch (_mbApiInterface.Player_GetPlayState())
                    {
                        case PlayState.Playing:
                            _currentTrackUrl = CurrentTrackUrl();

                            if (_mbApiInterface.Player_GetPosition() == 0)
                            {
                                LoadSavedPosition();
                            }

                            _playingTimer.Start();
                            break;
                        case PlayState.Paused:
                            _playingTimer.Stop();
                            SavePosition();
                            break;
                        case PlayState.Stopped:
                            _playingTimer.Stop();
                            break;
                    }

                    break;
                case NotificationType.PlayerScrobbleChanged:
                    SavePosition();
                    break;
                case NotificationType.TrackChanging:
                    _playingTimer.Stop();
                    break;
                case NotificationType.TrackChanged:
                    if (CurrentTrackUrl() == _currentTrackUrl)
                    {
                        return;
                    }

                    Thread.Sleep(500);
                    LoadSavedPosition();
                    _playingTimer.Start();
                    break;
            }
        }

        // return an array of lyric or artwork provider names this plugin supports
        // the providers will be iterated through one by one and passed to the RetrieveLyrics/ RetrieveArtwork function in order set by the user in the MusicBee Tags(2) preferences screen until a match is found
        public string[] GetProviders()
        {
            return null;
        }

        // return lyrics for the requested artist/title from the requested provider
        // only required if PluginType = LyricsRetrieval
        // return null if no lyrics are found
        public string RetrieveLyrics(string sourceFileUrl, string artist, string trackTitle, string album,
            bool synchronisedPreferred, string provider)
        {
            return null;
        }

        // return Base64 string representation of the artwork binary data from the requested provider
        // only required if PluginType = ArtworkRetrieval
        // return null if no artwork is found
        public string RetrieveArtwork(string sourceFileUrl, string albumArtist, string album, string provider)
        {
            //Return Convert.ToBase64String(artworkBinaryData)
            return null;
        }
    }
}