Bit/Byte Operation
#################

count1(*x)
*****************

Number of bits of `x` that have the value 1.

pack(b, array[]...)
*****************

Packs array values at b-bit intervals.

reverseBits(b, *x)
*****************

Reverses the lower `b` bits of `x`.

reverseBytes(b, *x)
*****************

Reverses the lower `b` bytes of `x`.

rotateL(b, *x)
*****************

Rotates the lower `b` bits of `x` to the left by 1 bit.

rotateL(b, n, *x)
*****************

Rotates the lower `b` bits of `x` to the left by `n` bits.

rotateR(b, *x)
*****************

Rotates the lower `b` bits of `x` to the right by 1 bit.

rotateR(b, n, *x)
*****************

Rotates the lower `b` bits of `x` to the right by `n` bits.

swap2(*x)
*****************

Swaps even and odd bytes of `x`.

swap4(*x)
*****************

Reverses the order of each 4 bytes of `x`.

swap8(*x)
*****************

Reverses the byte-order of `x`.

swapNib(*x)
*****************

Swaps the nibble of each byte of `x`.

unpack(array, x)
*****************

Divide the value of `x` into `n` elements of `b` bit width.

unpack(b, n, x)
*****************

Separate the value of `x` into `n` elements of `b` bit width.


