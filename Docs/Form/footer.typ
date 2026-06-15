#set page(
  margin: (
    left: 0cm,
    right: 0cm,
  )
)


#let page-footer = context {

  let current = counter(page).get().first()

  if current > 1 [
    #grid(
      columns: (auto, 1fr, auto),
      gutter: 0.5cm,

      [*Diogène - GDD*],

      [
        #rect(
          width: 100%,
          height: 0.12cm,
          fill: rgb("#98159d"),
        )
      ],

      [
         Page #(current - 1) / #(counter(page).final().first() - 1)
      ]
    )
  ]
}

#set page(
  margin: (
    left: 1cm,
    right: 1cm,
  )
)
