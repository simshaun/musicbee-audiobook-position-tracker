# Audiobook Position Tracker

This plugin will remember where you last listened.

# Huge Caveat!

I don't know of a foolproof way of making this apply to audiobooks only.
Currently, it remembers last position of anything played. My audiobooks
are in the m4b container format, so I *could* restrict this plugin to m4b files,
but then it would be useless for others who have audiobooks in other formats.

Because I use MusicBee for audiobooks only, this isn't a problem for me.
However, it may be annoying for you if you use MusicBee for music and other stuff.
e.g. I haven't tested this with podcasts or anything like that.

# Installation

Copy `bin/Release/mb_AudiobookPositionTracker.dll` to your MusicBee Plugins folder.

The MusicBee Plugins folder is likely at `C:\Program Files (x86)\MusicBee\Plugins`.

# How It Works

The plugin will store a text file in the same location as the track being played.
e.g. `C:\Music\Book.m4b` will result in a text file at `C:\Music\Book.m4b.position.txt`

It writes the position every second while the track is playing.