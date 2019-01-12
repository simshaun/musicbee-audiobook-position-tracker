# Audiobook Position Tracker

This plugin will remember where you last listened.

# Warning

I use MusicBee for audiobooks only and haven't tested this with a huge library.

# Installation

Copy `bin/Release/mb_AudiobookPositionTracker.dll` to your MusicBee Plugins folder.

The MusicBee Plugins folder is likely at `C:\Program Files (x86)\MusicBee\Plugins`.

# How It Works

The plugin will store a text file in the same location as the track being played.
e.g. `C:\Music\Book.m4b` will result in a text file at `C:\Music\Book.m4b.position.txt`

It writes the position every second while the track is playing.