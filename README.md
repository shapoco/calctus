# Calctus

a calculator for Windows.

![Screen Shot](img/ss.gif)

----

## Download

→ see [releases](https://github.com/shapoco/calctus/releases).

----

## Overview

- Displays the evaluation value of a formula in text format.
- Decimal, hexadecimal, and binary numbers can be mixed.
- The formula history can be referenced with the up key.
- The formula history can be modified, and if so, the calculation results are regenerated.
- Some built-in constants and built-in functions.

----

## Features

### Numeric Representations

|Description|Example|
|:--|--:|
|Decimal|`123`|
|Hexadecimal|`0x7b`, `0x7B`|
|Octadecimal|`0173`|
|Binary|`0b1`|

### Operators

|Category|Symbol|Precision|
|:--|:--|:--|
|Add, Sub, Mul, Div|`+`, `-`, `*`, `/`|`Decimal`|
|Integral Division|`//`|`Decimal`|
|Remainder|`%`|`Decimal`|
|Power|`^`|`Double`|
|Logical NOT|`~`|`Int64`|
|Logical AND|`&`|`Int64`|
|Logical OR|`\|`|`Int64`|
|Logical XOR|`+\|`|`Int64`|
|Logical shift|`<<`, `>>`|`Int64`|
|Arithmetic shift|`<<<`, `>>>`|`Int64`|


### Embedded Functions

|Category|Functions|Precision|
|:--|:--|:--|
|Exponential|`sqrt(x)`, `log(x)`, `log2(x)`, `log10(x)`, `clog2(x)`, `clog10(x)`|`Double`|
|Trigonometric|`sin(x)`, `cos(x)`, `tan(x)`,`asin(x)`, `acos(x)`, `atan(x)`, `atan2(y, x)`,`sinh(x)`:new:, `cosh(x)`:new:, `tanh(x)`:new:|`Double`|
|Round|`floor(x)`, `ceil(x)`, `trunc(x)`, `round(x)`|`Decimal`|
|E-series:new:|Rounding to the E-series value: `eXfloor(x)`, `eXceil(x)`, `eXround(x)`<br>Calculation of voltage divider resistance: `eXratiol(x)`, `eXratioh(x)`<br> (`X`=`3`, `6`, `12`, `24`, `48`, `96`, `192`)|`Decimal`|
|Others|`abs(x)`:new:, `sign(x)`, `max(a, b)`:new:, `min(a, b)`:new:|`Decimal`|

### Constants

|Symbol|Value|
|:--|--:|
|`PI`|`3.1415926535897931`|
|`E`|`2.7182818284590451`|
|`INT_MIN`|`-2,147,483,648`|
|`INT_MAX`|`2,147,483,647`|
|`UINT_MIN`|`0`|
|`UINT_MAX`|`4,294,967,295`|
|`LONG_MIN`:new:|`-9,223,372,036,854,775,808`|
|`LONG_MAX`:new:|`9,223,372,036,854,775,807`|
|`ULONG_MIN`:new:|`0`|
|`ULONG_MAX`:new:|`18,446,744,073,709,551,615`|
|~~`FLOAT_MIN`~~|~~`-3.40282347E+38f`~~|
|~~`FLOAT_MAX`~~|~~`3.40282347E+38f`~~|
|~~`DOUBLE_MIN`~~|~~`-1.7976931348623157E+308`~~|
|~~`DOUBLE_MAX`~~|~~`1.7976931348623157E+308`~~|
|`DECIMAL_MIN`:new:|`-79,228,162,514,264,337,593,543,950,335`|
|`DECIMAL_MAX`:new:|`79,228,162,514,264,337,593,543,950,335`|

### Variables

Variables can be assigned using the equal sign.

```
a = 2 [Return]
b = 3 [Return]
a * b [Return]
// --> Calctus answers 6.
```

### Keyboard Shortcut

|Key|Function|
|:--|:--|
|`Shift` + `Return`|Insert|
|`Shift` + `Delete` :new:|Delete current expression|
|`Ctrl` + `Shift` + `C` :new:|Copy all expressions and answers|
|`Ctrl` + `Shift` + `Del` :new:|Delete all expressions|
|`F9` :new:|Radix Mode = Auto|
|`F10` :new:|Radix Mode = Dec|
|`F11` :new:|Radix Mode = Hex|
|`F12` :new:|Radix Mode = Bin|

----

## Settings

![](img/settings_general.png)

![](img/settings_appearance.png)

![](img/settings_details.png)

----