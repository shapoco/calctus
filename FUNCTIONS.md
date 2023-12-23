# Built-In Functions

## Absolute/Sign

### `abs(*x)`

Absolute value of `x`

### `mag(x[]...)`

Magnitude of vector `x`.

### `sign(*x)`

Returns 1 for positives, -1 for negatives, 0 otherwise.

----
## Array

### `aggregate(array, func)`

Apply the aggregate function `func(a,b)` for the `array`.

### `all(array)`

Returns true if all `array` elements are true.

### `all(array, func)`

Returns true if tester function `func(x)` returns true for all elements of the `array`.

### `any(array)`

Returns true if at least one element is true.

### `any(array, func)`

Returns true if tester function `func(x)` returns true for at least one element of the `array`.

### `contains(array, *val)`

Returns whether the `array` contains `val`.

### `count(array, func)`

Count specific elements in the `array` using a tester function `func(x)`.

### `except(array0, array1)`

Returns the difference set of the two arrays.

### `extend(array, func, count)`

Extends the `array` using converter function `func(array)`.

### `filter(array, func)`

Filter the `array` using a tester function `func(x)`.

### `indexOf(array, *val)`

Returns the index of the first element in the `array` whose value matches `val`.

### `intersect(array0, array1)`

Returns the product set of the two arrays.

### `lastIndexOf(array, *val)`

Returns the index of the last element in the `array` whose value matches `val`.

### `len(array)`

Length of `array`

### `map(array, func)`

Map the `array` using a converter function `func(x)`.

### `range(start, stop)`

Returns an array consists of sequence of numbers greater than or equal to `start` and less than `stop`.

### `range(start, stop, step)`

Returns an array consists of sequence of numbers greater than or equal to `start` and less than `stop` with common difference `step`.

### `rangeInclusive(start, stop)`

Returns an array consists of sequence of numbers greater than or equal to `start` and less than or equal to `stop`.

### `rangeInclusive(start, stop, step)`

Returns an array consists of sequence of numbers greater than or equal to `start` and less than or equal to `stop` with common difference `step`.

### `reverseArray(array)`

Reverses the order of elements of `array`

### `sort(array)`

Sort the `array`.

### `sort(array, func)`

Sort the `array` using a evaluator function `func(x)`.

### `union(array0, array1)`

Returns the union of the two arrays.

### `unique(array)`

Returns an array of unique elements.

### `unique(array, func)`

Return unique elements using evaluator function `func(x)`.

----
## Assertion

### `assert(x)`

Raises an error if the `x` is false.

----
## Bit/Byte Operation

### `count1(*x)`

Number of bits of `x` that have the value 1.

### `pack(b, array[]...)`

Packs array values at b-bit intervals.

### `reverseBits(b, *x)`

Reverses the lower `b` bits of `x`.

### `reverseBytewise(*x)`

Reverses the order of bits of each byte of `x`.

### `rotateL(b, *x)`

Rotates left the lower `b` bits of `x`.

### `rotateR(b, *x)`

Rotates right the lower `b` bits of `x`.

### `swap2(*x)`

Swaps even and odd bytes of `x`.

### `swap4(*x)`

Reverses the order of each 4 bytes of `x`.

### `swap8(*x)`

Reverses the order of each 8 bytes of `x`.

### `swapNib(*x)`

Swaps the nibble of each byte of `x`.

### `unpack(b, x)`

Returns an array of `x` values divided into `b` bits.

----
## Cast

### `array(s)`

Converts string `s` to an array of character code.

### `rat(*x)`

Rational fraction approximation of `x`.

### `rat(*x, max)`

Rational fraction approximation of `x`.

### `real(*x)`

Converts the `x` to a real number.

### `str(array)`

Converts `array` to a string.

----
## Color

### `hsl2rgb(h, s, l)`

Convert from H, S, L to 24 bit color RGB value.

### `hsv2rgb(h, s, v)`

Converts from H, S, V to 24 bit RGB color value.

### `pack565(x, y, z)`

Packs the 3 values to an RGB565 color.

### `rgb(r, g, b)`

Generates 24 bit color value from R, G, B.

### `rgb(*rgb)`

Converts the `rgb` to web-color representation.

### `rgb2hsl(*rgb)`

Converts the 24 bit RGB color value to HSL.

### `rgb2hsv(*rgb)`

Converts the 24 bit RGB color value to HSV.

### `rgb2yuv(r, g, b)`

Converts R, G, B to 24 bit YUV color.

### `rgb2yuv(*rgb)`

Converts 24bit RGB color to 24 bit YUV.

### `rgbFrom565(*rgb)`

Upconverts RGB565 color to RGB888.

### `rgbTo565(*rgb)`

Downconverts RGB888 color to RGB565.

### `unpack565(*x)`

Unpacks the RGB565 color to 3 values.

### `yuv2rgb(y, u, v)`

Converts Y, U, V to 24 bit RGB color.

### `yuv2rgb(*yuv)`

Converts the 24 bit YUV color to 24 bit RGB.

----
## Date Time

### `fromDays(*x)`

Converts from days to epoch time.

### `fromHours(*x)`

Converts from hours to epoch time.

### `fromMinutes(*x)`

Converts from minutes to epoch time.

### `fromSeconds(*x)`

Converts from seconds to epoch time.

### `now()`

Current epoch time

### `toDays(*x)`

Converts from epoch time to days.

### `toHours(*x)`

Converts from epoch time to hours.

### `toMinutes(*x)`

Converts from epoch time to minutes.

### `toSeconds(*x)`

Converts from epoch time to seconds.

----
## Encoding

### `base64Dec(str)`

Decode Base64 to string.

### `base64DecBytes(str)`

Decode Base64 to byte-array.

### `base64Enc(str)`

Encode string to Base64.

### `base64EncBytes(bytes[]...)`

Encode byte-array to Base64.

### `urlDec(str)`

Unescape URL string.

### `urlEnc(str)`

Escape URL string.

### `utf8Dec(bytes[]...)`

Decode UTF8 byte sequence.

### `utf8Enc(str)`

Encode `str` to UTF8 byte sequence.

----
## E Series

### `esCeil(series, *x)`

Nearest E-series value greater than or equal to `x` (`series`=3, 6, 12, 24, 48, 96, or 192).

### `esFloor(series, *x)`

Nearest E-series value less than or equal to `x` (`series`=3, 6, 12, 24, 48, 96, or 192).

### `esRatio(series, *x)`

Two E-series resistor values that provide the closest value to the voltage divider ratio `x` (`series`=3, 6, 12, 24, 48, 96, or 192).

### `esRound(series, *x)`

Nearest E-series value (`series`=3, 6, 12, 24, 48, 96, or 192).

----
## Exponential

### `clog10(*x)`

Ceiling of common logarithm of `x`

### `clog2(*x)`

Ceiling of binary logarithm of `x`

### `exp(*x)`

Exponential of `x`

### `log(*x)`

Logarithm of `x`

### `log10(*x)`

Common logarithm of `x`

### `log2(*x)`

Binary logarithm of `x`

### `pow(*x, y)`

`y` power of `x`

### `sqrt(*x)`

Square root of `x`

----
## Gcd/Lcm

### `gcd(array...)`

Greatest common divisor of elements of the `array`.

### `lcm(array...)`

Least common multiple of elements of the `array`.

----
## Gray Code

### `fromGray(*x)`

Converts the value from gray-code to binary.

### `toGray(*x)`

Converts the value from binary to gray-code.

----
## Min/Max

### `max(x...)`

Maximum value of the arguments

### `min(x...)`

Minimum value of the arguments

----
## Parity/Ecc

### `eccDec(b, ecc, x)`

Checks ECC code (`b`: data width, `ecc`: ECC code, `x`: data). Returns: 0 = no error, positive value = position of 1-bit error, negative value = 2-bit error.

### `eccEnc(b, *x)`

Generates ECC code (`b`: data width, `x`: data).

### `eccWidth(*b)`

Width of ECC for `b`-bit data.

### `oddParity(*x)`

Odd parity of `x`.

### `xorReduce(*x)`

Reduction XOR of `x` (Same as even parity).

----
## Plotting

### `plot(func)`

Plot graph of `func(x)`.

----
## Prime Number

### `isPrime(*x)`

Returns whether `x` is prime or not.

### `prime(*x)`

`x`-th prime number.

### `primeFact(*x)`

Returns prime factors of `x`.

----
## Random

### `rand()`

Generates a random value between 0.0 and 1.0.

### `rand(min, max)`

Generates a random value between min and max.

### `rand32()`

Generates a 32bit random integer.

### `rand64()`

Generates a 64bit random integer.

----
## Representaion

### `bin(*x)`

Converts `x` to binary representation.

### `char(*x)`

Converts `x` to character representation.

### `datetime(*x)`

Converts `x` to datetime representation.

### `dec(*x)`

Converts `x` to decimal representation.

### `hex(*x)`

Converts `x` to hexdecimal representation.

### `kibi(*x)`

Converts `x` to binary prefixed representation.

### `oct(*x)`

Converts `x` to octal representation.

### `si(*x)`

Converts `x` to SI prefixed representation.

----
## Rounding

### `ceil(*x)`

Smallest integral value greater than or equal to `x`

### `floor(*x)`

Largest integral value less than or equal to `x`

### `round(*x)`

Nearest integer to `x`

### `trunc(*x)`

Integral part of `x`

----
## Solve

### `solve(func)`

Returns solutions of `func(x)=0` using Newton's Method.

### `solve(func, array)`

Returns solutions of `func(x)=0` using Newton's Method with initial value in `array`.

### `solve(func, min, max)`

Returns solutions of `func(x)=0` using Newton's Method with initial value between `min` and `max`.

----
## Sum/Average

### `ave(array...)`

Arithmetic mean of elements of the `array`.

### `geoMean(array...)`

Geometric mean of elements of the `array`.

### `harMean(array...)`

Harmonic mean of elements of the `array`.

### `invSum(array...)`

Inverse of the sum of the inverses. Composite resistance of parallel resistors.

### `sum(array...)`

Sum of elements of the `array`.

----
## Trigonometric

### `acos(*x)`

Arccosine

### `asin(*x)`

Arcsine

### `atan(*x)`

Arctangent

### `atan2(a, b)`

Arctangent of a / b

### `cos(*x)`

Cosine

### `cosh(*x)`

Hyperbolic cosine

### `sin(*x)`

Sine

### `sinh(*x)`

Hyperbolic sine

### `tan(*x)`

Tangent

### `tanh(*x)`

Hyperbolic tangent

----
