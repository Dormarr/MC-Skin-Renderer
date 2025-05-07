local sprite = app.sprite
if not sprite then
    app.alert("No active sprite found!")
    return
end

-- Ensure the sprite has a valid filename and is a PNG file
local skin_path = sprite.filename
if not skin_path or string.sub(skin_path, -4):lower() ~= ".png" then
    app.alert("Sprite must be saved as a .png file for the live preview to work!")
    return
end

local base_path = string.match(skin_path, "^(.*)[/\\]")

local function saveSkin()
  if not app.sprite then return end

  local ok, err = pcall(function()
    sprite:saveCopyAs(skin_path)
  end)

  if not ok then
    print("Failed to save skin: " .. tostring(err))
    return
  end

  if base_path then
    local trigger_file = io.open(base_path .. "/preview_trigger.txt", "w")
    if trigger_file then
      trigger_file:write(".")
      trigger_file:close()
    else
      print("Failed to write trigger file!")
    end
  else
    print("Could not determine the file directory.")
  end
end


sprite.events:on("change", saveSkin)