#import "Form/header.typ": page-header
#import "Form/footer.typ": page-footer

#let diagram = figure("diagram")
#let illustration = figure("illustration")

#show figure.where(kind: "diagram"): set figure(
  supplement: [Diagramme]
)

#show figure.where(kind: "illustration"): set figure(
  supplement: [Illustration]
)

#set text(font: "Noto Sans")

#set page(
  header: page-header,
  footer: page-footer,

  margin: (
    top: 1cm,
    bottom: 1cm,
  )
)

#import "Form/first-page.typ": first-page
#first-page

 #import "Form/table-of-content.typ": table-of-content
 #table-of-content