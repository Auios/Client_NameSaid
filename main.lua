local ____lualib = require("lualib_bundle")
local __TS__StringSplit = ____lualib.__TS__StringSplit
local __TS__ArrayFilter = ____lualib.__TS__ArrayFilter
local __TS__ArraySort = ____lualib.__TS__ArraySort
local __TS__StringIncludes = ____lualib.__TS__StringIncludes
local __TS__ArraySome = ____lualib.__TS__ArraySome
local __TS__StringSubstring = ____lualib.__TS__StringSubstring
local __TS__StringStartsWith = ____lualib.__TS__StringStartsWith
local __TS__StringReplace = ____lualib.__TS__StringReplace
local ____exports = {}
local ____Blockland = require("tslib.Blockland")
local bl = ____Blockland.default
bl.new("AudioProfile nameChatSound", {fileName = "Add-Ons/Client_NameSaid/nameSound.wav", description = "AudioGui", preload = true})
local function alnum(str)
    return table.concat(
        __TS__ArrayFilter(
            __TS__StringSplit(str, ""),
            function(____, c)
                local code = string.byte(c, 1) or 0 / 0
                return code >= 48 and code <= 57 or code >= 65 and code <= 90 or code >= 97 and code <= 122
            end
        ),
        ""
    )
end
bl.hook(
    "nameSaid",
    "clientCmdChatMessage",
    "before",
    function(data)
        local name = bl["pref::Player::NetName"]
        local nameSaidSound = bl["pref::Client::nameSaidSound"]
        if not nameSaidSound or not name then
            return
        end
        local message = data[8]
        local messageLower = string.lower(message)
        local nameLower = string.lower(name)
        local nameAlnum = alnum(name)
        local nameAlnumLower = string.lower(nameAlnum)
        local needles = {{original = name, lower = nameLower, len = #name}}
        if nameLower ~= nameAlnumLower then
            needles[#needles + 1] = {original = nameAlnum, lower = nameAlnumLower, len = #nameAlnum}
        end
        __TS__ArraySort(
            needles,
            function(____, a, b) return b.len - a.len end
        )
        if __TS__ArraySome(
            needles,
            function(____, needle) return __TS__StringIncludes(messageLower, needle.lower) end
        ) then
            bl.alxPlay("nameChatSound")
            local originalFullMessage = data[4]
            local originalUserMessage = data[8]
            local modifiedUserMessage = ""
            local lastIndex = 0
            do
                local i = 0
                while i < #originalUserMessage do
                    local foundMatch = false
                    for ____, needle in ipairs(needles) do
                        if __TS__StringStartsWith(
                            __TS__StringSubstring(messageLower, i),
                            needle.lower
                        ) then
                            modifiedUserMessage = modifiedUserMessage .. __TS__StringSubstring(originalUserMessage, lastIndex, i)
                            local originalMatch = __TS__StringSubstring(originalUserMessage, i, i + needle.len)
                            modifiedUserMessage = modifiedUserMessage .. ("<color:1589FF>" .. originalMatch) .. "<color:ffffff>"
                            i = i + needle.len
                            lastIndex = i
                            foundMatch = true
                            break
                        end
                    end
                    if not foundMatch then
                        i = i + 1
                    end
                end
            end
            if lastIndex > 0 then
                modifiedUserMessage = modifiedUserMessage .. __TS__StringSubstring(originalUserMessage, lastIndex)
                local finalFullMessage = __TS__StringReplace(originalFullMessage, originalUserMessage, modifiedUserMessage)
                data[4] = finalFullMessage
            end
        end
    end
)
return ____exports
