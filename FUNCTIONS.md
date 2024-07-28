# Built-In Functions

## Absolute/Sign

### `abs(*x)`

Absolute value of `x`

```
abs(-12.34) //--> 12.34
abs(0)      //--> 0
abs(56.78)  //--> 56.78
```
### `mag(x[]...)`

Magnitude of vector `x`

```
mag(3,4)   //--> 5
mag(3,4,5) //--> 7.071067812
```
### `sign(*x)`

Returns 1 for positives, -1 for negatives, 0 otherwise.

```
sign(-12.34) //--> -1
sign(0)      //--> 0
sign(56.78)  //--> 1
```
----
## Array

### `aggregate(array, func)`

Apply the aggregate function `func(a,b)` for the `array`.

```
aggregate([1,2,3,4,5],(a,b)=>a+b)
                         //--> 15
```
### `all(array)`

Returns true if all `array` elements are true.

```
all([true,false,true]) //--> false
all([true,true,true])  //--> true
```
### `all(array, func)`

Returns true if tester function `func(x)` returns true for all elements of the `array`.

```
all([2,3,5,7],isPrime)   //--> true
all([2,3,5,7,9],isPrime) //--> false
```
### `any(array)`

Returns true if at least one element is true.

```
any([false,false,false]) //--> false
any([false,true,false])  //--> true
```
### `any(array, func)`

Returns true if tester function `func(x)` returns true for at least one element of the `array`.

### `concat(array0, array1)`

Concatenate array0 and array1.

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

```
len([])            //--> 0
len([12,34,56,78]) //--> 4
len("Hello")       //--> 5
```
### `map(array, func)`

Map the `array` using a converter function `func(x)`.

### `range(start, stop)`

Returns an array consists of sequence of numbers greater than or equal to `start` and less than `stop`.

```
range(3,9) //--> [3, 4, 5, 6, 7, 8]
```
### `range(start, stop, step)`

Returns an array consists of sequence of numbers greater than or equal to `start` and less than `stop` with common difference `step`.

```
range(3,9,2) //--> [3, 5, 7]
```
### `rangeInclusive(start, stop)`

Returns an array consists of sequence of numbers greater than or equal to `start` and less than or equal to `stop`.

```
rangeInclusive(3,9) //--> [3, 4, 5, 6, 7, 8, 9]
```
### `rangeInclusive(start, stop, step)`

Returns an array consists of sequence of numbers greater than or equal to `start` and less than or equal to `stop` with common difference `step`.

```
rangeInclusive(3,9,2) //--> [3, 5, 7, 9]
```
### `reverseArray(array)`

Reverses the order of elements of `array`

```
reverseArray([12,3,4567,890]) //--> [890, 4567, 3, 12]
```
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

### `reverseBytes(b, *x)`

Reverses the lower `b` bytes of `x`.

### `rotateL(b, *x)`

Rotates the lower `b` bits of `x` to the left by 1 bit.

### `rotateL(b, n, *x)`

Rotates the lower `b` bits of `x` to the left by `n` bits.

### `rotateR(b, *x)`

Rotates the lower `b` bits of `x` to the right by 1 bit.

### `rotateR(b, n, *x)`

Rotates the lower `b` bits of `x` to the right by `n` bits.

### `swap2(*x)`

Swaps even and odd bytes of `x`.

### `swap4(*x)`

Reverses the order of each 4 bytes of `x`.

### `swap8(*x)`

Reverses the byte-order of `x`.

### `swapNib(*x)`

Swaps the nibble of each byte of `x`.

### `unpack(array, x)`

Divide the value of `x` into `n` elements of `b` bit width.

### `unpack(b, n, x)`

Separate the value of `x` into `n` elements of `b` bit width.

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

### `str(x)`

Converts `x` to a string.

----
## Color

### `hsl2rgb(h, s, l)`

Convert from H, S, L to 24 bit color RGB value.

### `hsv2rgb(h, s, v)`

Converts from H, S, V to 24 bit RGB color value.

### `pack565(x, y, z)`

Packs the 3 values to an RGB565 color.

### `rgb(*rgb)`

Converts the `rgb` to web-color representation.

### `rgb(r, g, b)`

Generates 24 bit color value from R, G, B.

### `rgb2hsl(*rgb)`

Converts the 24 bit RGB color value to HSL.

### `rgb2hsv(*rgb)`

Converts the 24 bit RGB color value to HSV.

### `rgb2yuv(*rgb)`

Converts 24bit RGB color to 24 bit YUV.

### `rgb2yuv(r, g, b)`

Converts R, G, B to 24 bit YUV color.

### `rgbFrom565(*rgb)`

Upconverts RGB565 color to RGB888.

### `rgbTo565(*rgb)`

Downconverts RGB888 color to RGB565.

### `unpack565(*x)`

Unpacks the RGB565 color to 3 values.

### `yuv2rgb(*yuv)`

Converts the 24 bit YUV color to 24 bit RGB.

### `yuv2rgb(y, u, v)`

Converts Y, U, V to 24 bit RGB color.

----
## Date Time

### `datetime(year, mon, day)`

Returns datetime from year, month, and day.

### `datetime(year, mon, day, hour, min, sec)`

Returns datetime from year, month, day, hour, minute, and second.

### `dayOfMonth(*t)`

Returns day component of datetime, expressed as 1..31.

### `dayOfWeek(*t)`

Returns day of week of datetime, expressed as 0 (Sunday)..6 (Saturday).

### `dayOfYear(*t)`

Returns day of year of datetime, expressed as 1..366.

### `fromDays(*x)`

Converts from days to epoch time.

```
fromDays(123.45) //--> #+123.10:48:00.000#
```
### `fromHours(*x)`

Converts from hours to epoch time.

```
fromHours(123.45) //--> #+5.3:27:00.000#
```
### `fromMinutes(*x)`

Converts from minutes to epoch time.

```
fromMinutes(123.45) //--> #+2:03:27.000#
```
### `fromSeconds(*x)`

Converts from seconds to epoch time.

```
fromSeconds(123.45) //--> #+0:02:03#
```
### `hourOf(*t)`

Returns hour component of datetime, expressed as 0..23.

### `minuteOf(*t)`

Returns minute component of datetime, expressed as 0..59.

### `monthOf(*t)`

Returns month component of datetime, expressed as 1..12.

### `now()`

Returns current datetime.

### `secondOf(*t)`

Returns second component of datetime, expressed as 0..60.

### `today()`

Returns datetime of today's 00:00:00.

### `toDays(*x)`

Converts from epoch time to days.

```
toDays(#+123.12:34:56.789#) //--> 123.524268391
```
### `toHours(*x)`

Converts from epoch time to hours.

```
toHours(#+123.12:34:56.789#) //--> 2964.582441389
```
### `toMinutes(*x)`

Converts from epoch time to minutes.

```
toMinutes(#+123.12:34:56.789#) //--> 177874.946483333
```
### `toSeconds(*x)`

Converts from epoch time to seconds.

```
toSeconds(#+123.12:34:56.789#) //--> 10672496.789
```
### `yearOf(*t)`

Returns year component of datetime.

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

### `clip(a, b, *x)`

Clips `x` to a range from `a` to `b`. Same as `max(a, min(b, x))`.

### `max(array...)`

Maximum value of elements of the `array`.

### `min(array...)`

Minimum value of elements of the `array`.

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

Converts UNIX time `x` to datetime representation.

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
## String

### `endsWith(*s, key)`

Whether string `s` ends with string `key` or not

### `join(sep, array[]...)`

Concatenates all elements of `array` using string `sep` as delimiter.

### `replace(*s, old, new)`

Replaces the string `old` in string `s` with the string `new`.

### `split(sep, s)`

Splits string `s` using string `sep` as delimiter.

### `startsWith(*s, key)`

Whether string `s` starts with string `key` or not

### `toLower(*s)`

Converts alphabetic characters in string `s` to lowercase.

### `toUpper(*s)`

Converts alphabetic characters in string `s` to uppercase.

### `trim(*s)`

Removes whitespace characters from both ends of string `s`.

### `trimEnd(*s)`

Removes whitespace characters from far end of string `s`.

### `trimStart(*s)`

Removes whitespace characters from near end of string `s`.

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
## System

### `isDebugBuild()`

Whether or not the running Calctus is a debug build.

### `version()`

Returns current version of Calctus.

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
