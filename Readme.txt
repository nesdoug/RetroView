RetroView

by Doug Fraker 2020
(.NET 4.5.2, windows, should work on non-windows
 systems with MONO)

An image processor with a more advanced posterize effect, with size limits.

I created this tool for 2 reasons.
1. I wanted to preview images in SNES direct color mode
2. I wanted to be able to do a posterize effect with dithering.

SNES direct color mode, however has uneven posterization 4,4,3. So, I
decided to separate the level of the effect over R,G,B and added
a min a max output because direct color can't output 100% white.

Then I added options for H,S,and V (B) which makes this a much more
powerful tool that can emulate various retro palettes (pseudo emulate)
by reducing hue count and saturation and brightness.

The dither is also unique because the user can adjust the amount of
dithering in the image. This is a variation of Floyd-Steinberg,
with a variable multiplier for each calculation (the box called
"dither"). 0 = off. 10 = standard.

The first 2 boxes (brightness and contrast) are for fine tuning the
image. (contrast is a contrast boost, it can't reduce contrast)
It seems the algorithm isn't perfect and frequently darkens the
image a little, so adding a tiny bit more brightness (and contrast to keep
black as black) can compensate for this.


Limits:
-only posterizes to 64 levels (256 means skip the posterization)
-auto-reduces the size of the image to max dimensions 256x240, which
 is a video game console dimension

The NES preset is nowhere near the actual palette. A better way to
get to an NES palette would be to quantize to a specific color set. But, 
I didn't add color reduction filters to this project.

The ZX Spectrum is also inaccurate (for the same reason). I seem to
get better results if I turn Contrast up +50 or more and set Dither
to a low level (2-3). Maybe also add brightness (the top left box).


Usage:
To get best results...usa another app to reduce and or crop an image to 
max dimensions 256x240 and adjust the brightness and contrast, and save
as .jpg, .gif, .bmp, or .png. Then load it into RetroView. RetroView can
auto-resize your picture and has brightness and contrast options, but
you will have far more control if you handle this manually.

If you are aiming at a specific console, use these dimensions (or less)
Atari 2600 - 160x192
NES - 256x240
Sega Master System - 256x224
SNES - 256x224
Sega Genesis - 256x224
Game Boy - 160x144
Game Gear - 160x144
GBA - 240x144
(resize in some other image / paint tool)

Usage, continued:
Load that image file to RetroView. Then, probably, you will mostly want to 
use presets, and then press the "process button". Then save the output
to a new image file. Type an extension for a specific image format.
(.jpg, .gif, .bmp, or .png).

For standard posterization with dither effect, only adjust the R,G,B levels
to the same amount (adjust the dither amount as needed. 0 = off. 10 = full.
11 and 12 are a little extra, added because I saw someone using a multiplier
between 1.1 and 1.2, but that produces an extra noisy image.)

How to achieve effects..

to reduce color count, either change H,S,B levels or R,G,B levels. I don't
recommend that you do both at the same time. Setting R,G,B to 2 each will
give 8 colors (2x2x2). Likewise, setting H,S,B to 2 each will give 8 colors.
But with that option, all the colors will be shades of red or cyan. You
could then adjust the minimum hue to 30 or 60 to shift the hues a bit.

You can increase or decrease the saturation and brightness by adjusting
the minimum and maximum amounts. Max saturation of 0 gives black and white.
Similarly, you can boost or reduce the R,G, or B balance by adjusting the
minimum and maximum amounts for them. Setting max to zero turns that
channel off.

Final Note:
Although you can preview an image in (for example) SNES direct color mode,
this tool can't output anything that can be imported into a game (such
as for homebrew game development). It is just for making interesting
posterized graphics.

If I could figure a way to quantize to 4 color, 16 color, and 256 color
palette sets, I could theoretically output to usable retro-game
graphics (.chr files). In the mean time, you could open a file
in GIMP or Photoshop, set mode to indexed, and reduce to the desired
color depth (4 for 2bpp, 16 for 4bpp, 256 for 8bpp), and then cut and
paste the image to YY-CHR (with it set to the appropriate console).

Don't try to output a dithered image, and then process it with GIMP /
Photoshop, then mode indexed with dithering. It gives poor results.
The double dithering is weird. Best results for 4bpp seem to be the
Gameboy preset, with contrast boosted up, medium dither. Then open in 
GIMP and mode indexed with no dithering. Cut and paste to YY-CHR.

...
The output size is fixed to 256x240 or less. If you recompile from source
you could adjust the MAX_WIDTH and MAX_HEIGHT constants to change this.
It's a visual studio project. C# desktop app with winforms, .NET.







