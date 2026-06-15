function Figure(fig)
  local img = nil

  -- Cherche l'image dans le contenu de la figure
  fig.content:walk {
    Image = function(i)
      img = i
    end
  }

  if img == nil then
    return nil
  end

  local kind = nil

  if img.classes:includes("diagram") then
    kind = "diagram"
  elseif img.classes:includes("illustration") then
    kind = "illustration"
  end

  if kind == nil then
    return nil
  end

  local caption = pandoc.utils.stringify(fig.caption)
  local src = img.src

  return pandoc.RawBlock("typst",
    '#figure(image("' .. src .. '"), caption: [' .. caption .. '], kind: "' .. kind .. '")'
  )
end