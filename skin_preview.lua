local sprite = app.activeSprite
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

-- Function to auto-save the sprite and trigger the preview update
local function autoSaveSkin()
    if not app.activeSprite then return end

    local ok, err = pcall(function()
        sprite:saveCopyAs(skin_path)
    end)

    if not ok then
        app.alert("Failed to save skin: " .. tostring(err))
        return
    end

    local base_path = string.match(skin_path, "^(.*)[/\\]")
    if base_path then
        local trigger_file = io.open(base_path .. "/preview_trigger.txt", "w")
        if trigger_file then
            trigger_file:write("update\n")
            trigger_file:close()
        else
            app.alert("Failed to write trigger file!")
        end
    else
        app.alert("Could not determine the file directory.")
    end
end

-- Hook into Aseprite’s event system to trigger on canvas edits
app.events:on("sitechange", autoSaveSkin)
sprite.events:on("change", autoSaveSkin)