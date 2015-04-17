First pass at a Banangrams solver. It currently just does a brute force of finding valid solutions given a dictionary by starting with the longest word that can be formed from the available letters. After starting with an initial word it iterates through all letters attempting to find a new word that intersects the existing letter. It uses a Trie implementation I found online to generate possible words.

It is far from optimal, but it works good enough to use in a game. After typing in the initial set of letters it goes into incremental solve mode where it asks for a single letter and then tries to do the minimal modification to fit it in.

## Example ##
```
Letters: krbheaeeradrnyuiaewwi
Find solutions: 00:00:00.284

.r..air.
.u..k...
.braid..
we.any..
..eh.ewe

....w..
....er.
.....e.
...a.r.
...r.u.
..akin.
yea.b..
..h.i..
....dew

.....w
...awe
r..k..
u..i..
brand.
yea.e.
..hie.
....r.

..b......
..r.i....
..e.r..y.
.awakened
.a.i.w...
uh.r.e...

..b.....
..r.y...
..e.e..i
.awkward
.a.e.i..
uh.e.r..
...n....
```

This is a board that was generated using all banangram letters, it took about 5 minutes to find this solution

Letters: aaaaaaaaaaaaabbbcccddddddeeeeeeeeeeeeeeeeeefffgggghhhiiiiiiiiiiiijjkklllllmmmnnnnnnnnooooooooooopppqqrrrrrrrrrsssssstttttttttuuuuuuvvvwwwxxyyyzz
```
Found:
..............w....
.......go....aeon..
.......ow...marrow.
.....s.delta.re....
.....u....iced.....
....employer.veep..
....mo..peso.arrive
....i......bore...x
..jut....aha.k.....
........abates....j
........add.a...t.u
........hi..ceiling
...f.....cosh.s.t..
...i...bran........
...n.g..eternal....
..additive.eye.....
..f.it.s....lox....
..f.g.......on.....
quiz........n......
..n................
quiz...............
..t................
..yuk..............
```