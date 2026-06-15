#let first-page = [
  #grid(
    rows: (1fr, 2fr, 2fr),
    

      [
      #align(horizon + center)[
        #text(size: 20pt, weight: "bold")[
          Game Design Document \
          Project Diogène
        ]
      ]
    ],

    [
      #align(horizon + center)[
        #image("../images/logo/logo.png", width: 100%, fit:"contain")
      ]
    ],

    [
      #align(horizon + center)[
        *Abominable Science* \
        www.abominablescience.ch
      ]
    ],
  )

  #v(0.5cm)
  #pagebreak()
]