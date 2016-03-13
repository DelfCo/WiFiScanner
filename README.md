# WiFiScanner
Shows you local WiFi access points.

Shows signal strength, and only shows you the strongest AP when it sees more than one with the same name.

This implementation uses converters to show channel number and signal strength (using the built-in wifi strength glyphs).

## To do list:
1. Change the algorithm for unique APs, to be band-aware and show duplicate APs if they're in different bands.
2. Redo the whole thing so it uses MVVM.
