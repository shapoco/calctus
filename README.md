# Calctus

Calctus (カルクタス) is a calculator application for Windows developed for engineers.

![Screen Shot](img/ss.png)

----

## Download

→ See [releases](https://github.com/shapoco/calctus/releases).

----

## Overview

- Syntax highlighting and auto-completion.
- Decimal, hexadecimal, and binary numbers can be mixed.
- SI prefixes, datetime formats, RGB color representation and several other formats are also available.
- The formula history can be modified, and the calculation results are immediately regenerated.
- Built-in constants and built-in functions. Constants and functions can also be user-defined.
- Numerical solution of equations by Newton's method.
- :new: High DPI support.

----

## Features

<!-- TODO: 表現形式と型は分けて書く -->

### Numeric Representations

|Representation|Example|
|:--|--:|
|Decimal|`123`|
|Hexadecimal|`0x7b`, `0x7B`|
|Octadecimal|`0173`|
|Binary|`0b1111011`|
|SI Prefixed|`123k`, `456u`|
|Binary Prefixed|`123ki`, `456Mi`|
|Fraction|`2$3`|
|Character|`'A'`|
|String|`"ABC"`|
|Date Time|`#2022/34/56 12:34:56#`|
|Web Color|`#123`, `#112233`|
|Boolean|`true`, `false`|
|Array|`[1, 2, 3]`|

### Operators

|Category|Symbol|Type|
|:--|:--|:--|
|Add, Sub, Mul, Div|`+`, `-`, `*`, `/`|`Decimal`|
|Integral Division|`//`|`Decimal`|
|Remainder|`%`|`Decimal`|
|Power|`^`|`Double`|
|Bit NOT|`~`|`Int64`|
|Bit AND|`&`|`Int64`|
|Bit OR|`\|`|`Int64`|
|XOR|`+\|`|`Int64`|
|Logical Shift|`<<`, `>>`|`Int64`|
|Arithmetic Shift|`<<<`, `>>>`|`Int64`|
|Bit/Part Select|`[ ]`, `[ : ]`|`Int64`|
|Compare|`>`, `>=`, `<`, `<=`, `==`, `!=`|`Decimal`|
|Logical NOT|`!`|`Boolean`|
|Logical AND|`&&`|`Boolean`|
|Logical OR|`\|\|`|`Boolean`|
|Conditional Operator|`? :`|`Boolean`|
|Range Operator|`..`, `..=`|`Array`|
|Arrow|`=>`|`Function`|

### Constants

|Symbol|Value|
|:--|--:|
|`PI`|`3.1415926535897932384626433833`|
|`E`|`2.7182818284590452353602874714`|
|`INT_MIN`|`-2,147,483,648`|
|`INT_MAX`|`2,147,483,647`|
|`UINT_MIN`|`0`|
|`UINT_MAX`|`4,294,967,295`|
|`LONG_MIN`|`-9,223,372,036,854,775,808`|
|`LONG_MAX`|`9,223,372,036,854,775,807`|
|`ULONG_MIN`|`0`|
|`ULONG_MAX`|`18,446,744,073,709,551,615`|
|`DECIMAL_MIN`|`-79,228,162,514,264,337,593,543,950,335`|
|`DECIMAL_MAX`|`79,228,162,514,264,337,593,543,950,335`|

User-defined constants can also be used. 

### Built-In Functions

See [Built-In Functions](FUNCTIONS.md) for details.

<!-- START_OF_BUILT_IN_FUNCTION_TABLE -->
Now Calctus has 165 built-in functions.

|Category|Functions|
|:--:|:--|
|[Absolute/Sign](./FUNCTIONS.md#absolutesign)|[abs(*x)](./FUNCTIONS.md#absx), [mag(x[]...)](./FUNCTIONS.md#magx), [sign(*x)](./FUNCTIONS.md#signx)|
|[Array](./FUNCTIONS.md#array)|[aggregate(array, func)](./FUNCTIONS.md#aggregatearray-func), [all(array)](./FUNCTIONS.md#allarray), [all(array, func)](./FUNCTIONS.md#allarray-func), [any(array)](./FUNCTIONS.md#anyarray), [any(array, func)](./FUNCTIONS.md#anyarray-func), [concat(array0, array1)](./FUNCTIONS.md#concatarray0-array1), [contains(array, *val)](./FUNCTIONS.md#containsarray-val), [count(array, func)](./FUNCTIONS.md#countarray-func), [except(array0, array1)](./FUNCTIONS.md#exceptarray0-array1), [extend(array, func, count)](./FUNCTIONS.md#extendarray-func-count), [filter(array, func)](./FUNCTIONS.md#filterarray-func), [indexOf(array, *val)](./FUNCTIONS.md#indexofarray-val), [intersect(array0, array1)](./FUNCTIONS.md#intersectarray0-array1), [lastIndexOf(array, *val)](./FUNCTIONS.md#lastindexofarray-val), [len(array)](./FUNCTIONS.md#lenarray), [map(array, func)](./FUNCTIONS.md#maparray-func), [range(start, stop)](./FUNCTIONS.md#rangestart-stop), [range(start, stop, step)](./FUNCTIONS.md#rangestart-stop-step), [rangeInclusive(start, stop)](./FUNCTIONS.md#rangeinclusivestart-stop), [rangeInclusive(start, stop, step)](./FUNCTIONS.md#rangeinclusivestart-stop-step), [reverseArray(array)](./FUNCTIONS.md#reversearrayarray), [sort(array)](./FUNCTIONS.md#sortarray), [sort(array, func)](./FUNCTIONS.md#sortarray-func), [union(array0, array1)](./FUNCTIONS.md#unionarray0-array1), [unique(array)](./FUNCTIONS.md#uniquearray), [unique(array, func)](./FUNCTIONS.md#uniquearray-func)|
|[Assertion](./FUNCTIONS.md#assertion)|[assert(x)](./FUNCTIONS.md#assertx)|
|[Bit/Byte Operation](./FUNCTIONS.md#bitbyte-operation)|[count1(*x)](./FUNCTIONS.md#count1x), [pack(b, array[]...)](./FUNCTIONS.md#packb-array), [reverseBits(b, *x)](./FUNCTIONS.md#reversebitsb-x), [reverseBytes(b, *x)](./FUNCTIONS.md#reversebytesb-x), [rotateL(b, *x)](./FUNCTIONS.md#rotatelb-x), [rotateL(b, n, *x)](./FUNCTIONS.md#rotatelb-n-x), [rotateR(b, *x)](./FUNCTIONS.md#rotaterb-x), [rotateR(b, n, *x)](./FUNCTIONS.md#rotaterb-n-x), [swap2(*x)](./FUNCTIONS.md#swap2x), [swap4(*x)](./FUNCTIONS.md#swap4x), [swap8(*x)](./FUNCTIONS.md#swap8x), [swapNib(*x)](./FUNCTIONS.md#swapnibx), [unpack(array, x)](./FUNCTIONS.md#unpackarray-x), [unpack(b, n, x)](./FUNCTIONS.md#unpackb-n-x)|
|[Cast](./FUNCTIONS.md#cast)|[array(s)](./FUNCTIONS.md#arrays), [rat(*x)](./FUNCTIONS.md#ratx), [rat(*x, max)](./FUNCTIONS.md#ratx-max), [real(*x)](./FUNCTIONS.md#realx), [str(x)](./FUNCTIONS.md#strx)|
|[Color](./FUNCTIONS.md#color)|[hsl2rgb(h, s, l)](./FUNCTIONS.md#hsl2rgbh-s-l), [hsv2rgb(h, s, v)](./FUNCTIONS.md#hsv2rgbh-s-v), [pack565(x, y, z)](./FUNCTIONS.md#pack565x-y-z), [rgb(*rgb)](./FUNCTIONS.md#rgbrgb), [rgb(r, g, b)](./FUNCTIONS.md#rgbr-g-b), [rgb2hsl(*rgb)](./FUNCTIONS.md#rgb2hslrgb), [rgb2hsv(*rgb)](./FUNCTIONS.md#rgb2hsvrgb), [rgb2yuv(*rgb)](./FUNCTIONS.md#rgb2yuvrgb), [rgb2yuv(r, g, b)](./FUNCTIONS.md#rgb2yuvr-g-b), [rgbFrom565(*rgb)](./FUNCTIONS.md#rgbfrom565rgb), [rgbTo565(*rgb)](./FUNCTIONS.md#rgbto565rgb), [unpack565(*x)](./FUNCTIONS.md#unpack565x), [yuv2rgb(*yuv)](./FUNCTIONS.md#yuv2rgbyuv), [yuv2rgb(y, u, v)](./FUNCTIONS.md#yuv2rgby-u-v)|
|[Date Time](./FUNCTIONS.md#date-time)|[datetime(year, mon, day)](./FUNCTIONS.md#datetimeyear-mon-day), [datetime(year, mon, day, hour, min, sec)](./FUNCTIONS.md#datetimeyear-mon-day-hour-min-sec), [dayOfMonth(*t)](./FUNCTIONS.md#dayofmontht), [dayOfWeek(*t)](./FUNCTIONS.md#dayofweekt), [dayOfYear(*t)](./FUNCTIONS.md#dayofyeart), [fromDays(*x)](./FUNCTIONS.md#fromdaysx), [fromHours(*x)](./FUNCTIONS.md#fromhoursx), [fromMinutes(*x)](./FUNCTIONS.md#fromminutesx), [fromSeconds(*x)](./FUNCTIONS.md#fromsecondsx), [hourOf(*t)](./FUNCTIONS.md#houroft), [minuteOf(*t)](./FUNCTIONS.md#minuteoft), [monthOf(*t)](./FUNCTIONS.md#monthoft), [now()](./FUNCTIONS.md#now), [secondOf(*t)](./FUNCTIONS.md#secondoft), [today()](./FUNCTIONS.md#today), [toDays(*x)](./FUNCTIONS.md#todaysx), [toHours(*x)](./FUNCTIONS.md#tohoursx), [toMinutes(*x)](./FUNCTIONS.md#tominutesx), [toSeconds(*x)](./FUNCTIONS.md#tosecondsx), [yearOf(*t)](./FUNCTIONS.md#yearoft)|
|[E Series](./FUNCTIONS.md#e-series)|[esCeil(series, *x)](./FUNCTIONS.md#esceilseries-x), [esFloor(series, *x)](./FUNCTIONS.md#esfloorseries-x), [esRatio(series, *x)](./FUNCTIONS.md#esratioseries-x), [esRound(series, *x)](./FUNCTIONS.md#esroundseries-x)|
|[Encoding](./FUNCTIONS.md#encoding)|[base64Dec(str)](./FUNCTIONS.md#base64decstr), [base64DecBytes(str)](./FUNCTIONS.md#base64decbytesstr), [base64Enc(str)](./FUNCTIONS.md#base64encstr), [base64EncBytes(bytes[]...)](./FUNCTIONS.md#base64encbytesbytes), [urlDec(str)](./FUNCTIONS.md#urldecstr), [urlEnc(str)](./FUNCTIONS.md#urlencstr), [utf8Dec(bytes[]...)](./FUNCTIONS.md#utf8decbytes), [utf8Enc(str)](./FUNCTIONS.md#utf8encstr)|
|[Exponential](./FUNCTIONS.md#exponential)|[clog10(*x)](./FUNCTIONS.md#clog10x), [clog2(*x)](./FUNCTIONS.md#clog2x), [exp(*x)](./FUNCTIONS.md#expx), [log(*x)](./FUNCTIONS.md#logx), [log10(*x)](./FUNCTIONS.md#log10x), [log2(*x)](./FUNCTIONS.md#log2x), [pow(*x, y)](./FUNCTIONS.md#powx-y), [sqrt(*x)](./FUNCTIONS.md#sqrtx)|
|[Gcd/Lcm](./FUNCTIONS.md#gcdlcm)|[gcd(array...)](./FUNCTIONS.md#gcdarray), [lcm(array...)](./FUNCTIONS.md#lcmarray)|
|[Gray Code](./FUNCTIONS.md#gray-code)|[fromGray(*x)](./FUNCTIONS.md#fromgrayx), [toGray(*x)](./FUNCTIONS.md#tograyx)|
|[Min/Max](./FUNCTIONS.md#minmax)|[clip(a, b, *x)](./FUNCTIONS.md#clipa-b-x), [max(array...)](./FUNCTIONS.md#maxarray), [min(array...)](./FUNCTIONS.md#minarray)|
|[Parity/Ecc](./FUNCTIONS.md#parityecc)|[eccDec(b, ecc, x)](./FUNCTIONS.md#eccdecb-ecc-x), [eccEnc(b, *x)](./FUNCTIONS.md#eccencb-x), [eccWidth(*b)](./FUNCTIONS.md#eccwidthb), [oddParity(*x)](./FUNCTIONS.md#oddparityx), [xorReduce(*x)](./FUNCTIONS.md#xorreducex)|
|[Plotting](./FUNCTIONS.md#plotting)|[plot(func)](./FUNCTIONS.md#plotfunc)|
|[Prime Number](./FUNCTIONS.md#prime-number)|[isPrime(*x)](./FUNCTIONS.md#isprimex), [prime(*x)](./FUNCTIONS.md#primex), [primeFact(*x)](./FUNCTIONS.md#primefactx)|
|[Random](./FUNCTIONS.md#random)|[rand()](./FUNCTIONS.md#rand), [rand(min, max)](./FUNCTIONS.md#randmin-max), [rand32()](./FUNCTIONS.md#rand32), [rand64()](./FUNCTIONS.md#rand64)|
|[Representaion](./FUNCTIONS.md#representaion)|[bin(*x)](./FUNCTIONS.md#binx), [char(*x)](./FUNCTIONS.md#charx), [datetime(*x)](./FUNCTIONS.md#datetimex), [dec(*x)](./FUNCTIONS.md#decx), [hex(*x)](./FUNCTIONS.md#hexx), [kibi(*x)](./FUNCTIONS.md#kibix), [oct(*x)](./FUNCTIONS.md#octx), [si(*x)](./FUNCTIONS.md#six)|
|[Rounding](./FUNCTIONS.md#rounding)|[ceil(*x)](./FUNCTIONS.md#ceilx), [floor(*x)](./FUNCTIONS.md#floorx), [round(*x)](./FUNCTIONS.md#roundx), [trunc(*x)](./FUNCTIONS.md#truncx)|
|[Solve](./FUNCTIONS.md#solve)|[solve(func)](./FUNCTIONS.md#solvefunc), [solve(func, array)](./FUNCTIONS.md#solvefunc-array), [solve(func, min, max)](./FUNCTIONS.md#solvefunc-min-max)|
|[String](./FUNCTIONS.md#string)|[endsWith(*s, key)](./FUNCTIONS.md#endswiths-key), [join(sep, array[]...)](./FUNCTIONS.md#joinsep-array), [replace(*s, old, new)](./FUNCTIONS.md#replaces-old-new), [split(sep, s)](./FUNCTIONS.md#splitsep-s), [startsWith(*s, key)](./FUNCTIONS.md#startswiths-key), [toLower(*s)](./FUNCTIONS.md#tolowers), [toUpper(*s)](./FUNCTIONS.md#touppers), [trim(*s)](./FUNCTIONS.md#trims), [trimEnd(*s)](./FUNCTIONS.md#trimends), [trimStart(*s)](./FUNCTIONS.md#trimstarts)|
|[Sum/Average](./FUNCTIONS.md#sumaverage)|[ave(array...)](./FUNCTIONS.md#avearray), [geoMean(array...)](./FUNCTIONS.md#geomeanarray), [harMean(array...)](./FUNCTIONS.md#harmeanarray), [invSum(array...)](./FUNCTIONS.md#invsumarray), [sum(array...)](./FUNCTIONS.md#sumarray)|
|[System](./FUNCTIONS.md#system)|[isDebugBuild()](./FUNCTIONS.md#isdebugbuild), [version()](./FUNCTIONS.md#version)|
|[Trigonometric](./FUNCTIONS.md#trigonometric)|[acos(*x)](./FUNCTIONS.md#acosx), [asin(*x)](./FUNCTIONS.md#asinx), [atan(*x)](./FUNCTIONS.md#atanx), [atan2(a, b)](./FUNCTIONS.md#atan2a-b), [cos(*x)](./FUNCTIONS.md#cosx), [cosh(*x)](./FUNCTIONS.md#coshx), [sin(*x)](./FUNCTIONS.md#sinx), [sinh(*x)](./FUNCTIONS.md#sinhx), [tan(*x)](./FUNCTIONS.md#tanx), [tanh(*x)](./FUNCTIONS.md#tanhx)|
<!-- END_OF_BUILT_IN_FUNCTION_TABLE -->

### Variables

Variables can be assigned using the equal sign.

```c++
a = 2
b = 3
a * b // --> 6.
```

JavaScript-style multiple assignment is available.

```c++
[a,b,c]=[10,20,30]
a // --> 10
b // --> 20
c // --> 30
```

### User Defined Function 

User functions can be defined using the `def` keyword.

```c++
def f(x) = x^2
f(3) // --> 9.
```

### Lambda Function

Rust-style lambda function is available.

```c++
f=(a,b)=>a*b
f(3,4) // --> 12.
```

If there is only one argument, the parentheses can be omitted

```c++
f=x=>x^2
f(3) // --> 9.
```

### Solve Function (Newton-Raphson method) 

Use the `solve` function to solve equations numerically by Newton's method.

```c++
def f(x)=x^2-2
solve(f) // --> [-1.414213562, 1.414213562].
solve(x=>x^2-2) // This gives the same result.
```

By default, the Newton's method is performed based on automatically generated initial values. Therefore, it may produce inaccurate results if the solution is concentrated in a small area or exists far from the origin.

In such cases, the 2nd argument can be given an initial value.

```c++
solve(sin,314) // --> 314.159265359.
```

That initial value can also be given as an array.

```c++
solve(sin,[-314,314]) // --> [-314.159265359, 314.159265359].
```

By providing the 2nd and 3rd arguments at the same time, a range of initial values can be specified. In this case, 101 values between these ranges are used as initial values.

```c++
solve(sin,-5,5) // --> [-3.141592654, 0, 3.141592654].
```

:warning: Note that the solution obtained using the `solve` function is an approximation and does not necessarily correspond to the analytical solution.

### Part-selection 

Verilog-style part selection is available for arrays and scalar values.

```c++
a=[1,2,3]
a[1]   // --> 2
a[1]=5
a      // --> [1, 5, 3]
```

```c++
x=0x1234
x[11:4]      // --> 0x23.
x[11:4]=0xab
x            // --> 0x1ab4.
```

Negative indexes represents distance from the end of the array.

```c++
array=[1,2,3,4,5]
array[-2]    // --> 4.
array[-3:-1] // --> [3, 4, 5].
```

### Range operator and range function

Rust-style range operators can be used to generate sequences of numbers.

```c++
1..5  // --> [1, 2, 3, 4]
1..=5 // --> [1, 2, 3, 4, 5]
```

Python-style range functions can also be used.

```c++
range(1,5)              // --> [1, 2, 3, 4]
range(1,5,0.5)          // --> [1, 1.5, 2, 2.5, 3, 3.5, 4, 4.5]
rangeInclusive(1,5)     // --> [1, 2, 3, 4, 5]
rangeInclusive(1,5,0.5) // --> [1, 1.5, 2, 2.5, 3, 3.5, 4, 4.5, 5]
```

### Vectorization

Many unary and binary operators can be applied to arrays to perform operations on its individual elements.

```
-[1,2,3]        // --> [-1, -2, -3]
[1,2,3]*4       // --> [4, 8, 12]
[1,2,3]+[4,5,6] // --> [5, 7, 9]
```

Also functions with arguments marked with an asterisk can be vectorized by supplying an array for the argument.

For example, the power function `pow(*x, y)` can take an array as its `x` argument. `pow([1,2,3],3)` is equivalent to `[pow(1,3),pow(2,3),pow(3,3)]`.

```
pow(2,3)        // --> 8
pow([1,2,3],3)  // --> [1, 8, 27]
prime(0..5)     // --> [2, 3, 5, 7, 11]
```

### Omission of Opening Parentheses 

The opening parenthesis at the beginning of a line can be omitted.

```c++
1+2)*3 // --> 9
```

### Auto-Completion

![](img/auto_completion.gif)

### Exponent Adjustment

Exponent-part of E-notation and SI prefix can be adjusted using Alt + arrow keys.

![](img/exp_adj.gif)

### Assertion

![](img/assertion.gif)

### RPN Operations

If only an operator is entered, the expression is "popped" from the history and combined with that operator.

If you use RPN operation, it is recommended to turn off the automatic input of "ans" in the settings.

```c++
1 [Return]
2 [Return]
+ [Return] // --> "1" and "2" replaced with "1+2"
3 [Return]
4 [Return]
+* [Return] // --> "1+2", "3" and "4" replaced with "(1+2)*(3+4)"
```

![](img/rpn_ops.gif)

### External Script Call as Functions

Calctus can call scripts such as Python as functions.

Function arguments are passed to the script as command line arguments, and the standard output of the script is returned to Calctus.

1. Open the `Scripts` tab in the Calctus Settings dialog.
2. Check `Enable External Script Functions`.
3. If you want to specify the folder where you want to place the scripts, click the `Change` button to specify the folder.
4. Click the `Open` button. If asked if you want to create the folder, click `Yes`.
5. Place a python script `add.py` like the following in the folder.
    ```python
    import sys
    a = float(sys.argv[1])
    b = float(sys.argv[2])
    print(a + b)
    ```
6. After closing the settings dialog, the `add` function is now available.

If you wish to use a scripting language other than Python, please register the extension and interpreter in the `Scripts` tab of the Settings dialog.

### Graph Plotting

`plot(func)` can be used to draw a graph. `func` is a function that takes one argument. The value of the argument is plotted on the horizontal axis and the return value of the function on the vertical axis.

:warning: This feature is experimental and may be removed in future versions.

![](img/graph.png)

### Keyboard Shortcut

|Key|Function|
|:--|:--|
|`Shift` + `Return`|Insert a new line before current expression|
|`Shift` + `Delete`|Delete current expression|
|`Ctrl` + `S`|Overwrite current sheet|
|`Ctrl` + `Z`|Undo|
|`Ctrl` + `Y`|Redo|
|`Ctrl` + `X`|Cut|
|`Ctrl` + `C`|Copy|
|`Ctrl` + `Shift` + `C`|Copy all expressions and answers|
|`Ctrl` + `V`|Paste (accepts multiple lines)|
|`Ctrl` + `Shift` + `V`|Paste with formatting|
|`Ctrl` + `Shift` + `N`|Insert current time|
|`Ctrl` + `Shift` + `Del`|Delete all expressions|
|`Ctrl` + `Shift` + `Up`/`Down`|Item move up/down|
|`Ctrl` + `Home`|Select first expression|
|`Ctrl` + `End`|Select last expression|
|`Alt` ( + `Shift` ) + `Left`/`Right`|Move decimal point|
|`Alt` + `Up`/`Down`|Change SI prefix|
|`F1`|Help|
|`F5`|Recalculation|
|`F8`|Radix Mode = Auto|
|`F9`|Radix Mode = Dec|
|`F10`|Radix Mode = Hex|
|`F11`|Radix Mode = Bin|
|`F12`|Radix Mode = SI Prefixed|

----

## Settings

![](img/settings_general.png)

![](img/settings_appearance.png)

![](img/settings_input.png)

![](img/settings_format.png)

![](img/settings_calculation.png)

![](img/settings_definitions.png)

![](img/settings_scripts.png)

----
