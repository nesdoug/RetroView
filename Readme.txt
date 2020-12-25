RetroView 
ver 1.1

by Doug Fraker 2020
(.NET 4.5.2, windows, should work on non-windows
 systems with MONO)

An image processor with a more advanced posterize effect, with size limits.


update 1.1
-added ordered dithering (bayer 8x8)
-added output resizing
-allow contrast to be negative (-100 to +100)
-added zoom feature (for very small images)
-fixed bug, incorrectly calculating brightness, images were too dark
-fixed bug, sometimes imported image was too small
-edited and added presets



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

Most of the game presets are nowhere near accurate. Too many colors on
screen at once. Perhaps in the future I can reduce the total # of colors
to more reasonable 4 or 16 total, rather than thousands.

The NES preset is nowhere near the actual palette. A better way to
get to an NES palette would be to quantize to a specific color set. But, 
that would be an entirely different project.

The ZX Spectrum is also inaccurate (for the same reason).


Usage:

To get best results...use another app to reduce and or crop an image to 
max dimensions 256x240 and adjust the brightness and contrast, and save
as .jpg, .gif, .bmp, or .png. Then load it into RetroView. RetroView can
auto-resize your picture and has brightness and contrast options, but
you will have far more control if you handle this manually.

Load that image file to RetroView. Then, probably, you will mostly want to 
use presets, and then press the "process button". Then save the output
to a new image file. Type an extension for a specific image format.
(.jpg, .gif, .bmp, or .png).

For standard posterization with dither effect, only adjust the R,G,B levels
to the same amount (adjust the dither amount as needed. 0 = off. 10 = full.
11 and 12 are a little extra, added because I saw someone using a multiplier
between 1.1 and 1.2, but that produces an extra noisy image.) I personally
think that using dither at 6-8 produces better results.


How to achieve effects..

To reduce color count, either change H,S,B levels or R,G,B levels. I don't
recommend that you do both at the same time. Setting R,G,B to 2 each will
give 8 colors (2x2x2). Likewise, setting H,S,B to 2 each will give 8 colors.
But with that option, all the colors will be shades of red or cyan.

You can increase or decrease the saturation and brightness by adjusting
the minimum and maximum amounts. Max saturation of 0 gives black and white.
Similarly, you can boost or reduce the R,G, or B balance by adjusting the
minimum and maximum amounts for them. Setting max to zero turns that
channel off.

To get closer to an actual console, you could first process with RetroView,
save to file. Then open in Photoshop/GIMP and change the color mode to
indexed with 4 or 16 colors. If you dithered in RetroView, don't process 
it with GIMP / Photoshop, then mode indexed with dithering. It gives poor 
results. The double dithering is weird.


Final Note:

Although you can preview an image in (for example) SNES direct color mode,
this tool can't output anything that can be imported into a game (such
as for homebrew game development). It is just for making interesting
posterized graphics.










