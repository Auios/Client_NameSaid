new AudioProfile(nameChatSound)
{
    fileName = "./nameSound.wav";
    description = AudioGui;
    preload = true;
};

package nameSaid
{
    function clientCmdChatMessage(%wat,%lol,%m,%msg,%a0,%a1,%a2,%a3,%a4,%a5,%a6,%a7,%a8,%a9)
    {

        %name = $pref::Player::NetName;
        %lanName = $pref::player::LanName;

        if($Pref::Client::nameSaidSound && (striPos(%a3,%name) >= 0 || striPos(%a3, %lanName) >= 0) || hasNickName(%a3))
        {
            alxPlay(nameChatSound);
            %len1 = strLen(%a3);

            if(hasNickName(%a3))
            {
                %nickpos = getWord(findNickNamePos(%a3),0);
                %nick = getWord(findNickNamePos(%a3,1));
            }
            else if(striPos(%a3,%name) >= 0)
            {
                %nickPos = striPos(%a3,%name);
                %nick = %name;
            }
            else if(striPos(%a3,%lanName) >= 0)
            {
                %nickPos = striPos(%a3,%lanName);
                %nick = %lanName;
            }

            %a3sPos = strPos(%msg, %a3);
            %len2 = strLen(%nick);

            %firstPart = getSubStr(%msg,0,%a3sPos); //Before Message
            %secondPart = getSubStr(%a3,0,%nickPos); //Message until Name
            %thirdPart = getSubStr(%a3,%nickPos,%len2); //Name
            %fourthPart = getSubStr(%a3,%nickPos + %len2, %len1); //After Name till end

            %msg = %firstPart @ %secondPart @ "<color:1589FF>" @ %thirdPart @ "<color:ffffff>" @ %fourthPart;
        }
        parent::clientCmdChatMessage(%wat,%lol,%m,%msg,%a0,%a1,%a2,%a3,%a4,%a5,%a6,%a7,%a8,%a9);
    }
};
activatePackage(nameSaid);

//Some of this tooken from Client_Chatsound by Chrono

if($Pref::Client::nameSaidSound $= "")
{
    $Pref::Client::nameSaidSound = true;
}

function toggleNameSaidSound(%val)
{
    if(%val)
    {
        $Pref::Client::nameSaidSound = $Pref::Client::nameSaidSound ? false : true;
        clientCmdServerMessage('',"Name Said Sound enabled: " @ $Pref::Client::nameSaidSound);
    }
}

function AddBind(%division, %name, %command)
{
    for(%i=0;%i<$remapCount;%i++)
    {
        if($remapDivision[%i] $= %division)
        {
            %foundDiv = 1;
            continue;
        }
        if(%foundDiv && $remapDivision[%i] !$= "")
        {
            %position = %i;
            break;
        }
    }
    if(!%foundDiv)
    {
        error("Division not found: " @ %division);
        return;
    }
    if(!%position)
    {
        $remapName[$remapCount] = %name;
        $remapCmd[$remapCount] = %command;
        $remapCount++;
        return;
    }
    for(%i=$remapCount;%i>%position;%i--)
    {
        $remapDivision[%i] = $remapDivision[%i - 1];
        $remapName[%i] = $remapName[%i - 1];
        $remapCmd[%i] = $remapCmd[%i - 1];
    }
    $remapDivision[%position] = "";
    $remapName[%position] = %name;
    $remapCmd[%position] = %command;
    $remapCount++;
}

if(!$addednameSaidSoundBind)
{
    AddBind("Communication","Toggle Name Said Sound","togglenameSaidSound");
    $addednameSaidSoundBind = true;
}


//Nicknames
function NSAddNickName(%nick)
{
    clientCmdServerMessage('',"\c6Name Said Sound: Added Nick Name: " @ %nick);
    $Pref::Client::NameSaidNickNames = addItemToList($Pref::Client::NameSaidNickNames,%nick);

    export("$Pref::Client::*","config/client/prefs.cs");
}

function NSRemoveNickName(%nick)
{
    $Pref::Client::NameSaidNickNames = removeItemFromList($Pref::Client::NameSaidNickNames,%nick);

    export("$Pref::Client::*","config/client/prefs.cs");
    clientCmdServerMessage('',"\c6Name Said Sound: Removed Nick Name: " @ %nick);
}

function hasNickName(%text)
{
    %count = getWordCount(%text);
    %list = $Pref::Client::NameSaidNickNames;

    for(%i = 0; %i < %count; %i++)
    {
        %word[%i] = getWord(%text,%i);
        if(hasItemOnList(%list,%word[%i]))
        {
            return true;
        }
    }
    return false;
}


function findNickNamePos(%text)
{
    %count = getWordCount(%text);
    %list = $Pref::Client::NameSaidNickNames;

    for(%i = 0; %i < %count; %i++)
    {
        %word[%i] = getWord(%text,%i);
        if(hasItemOnList(%list,%word[%i]))
        {
            break;
        }
    }

    %nick = %word[%i];
    %pos = striPos(%text, %nick);

    return %pos TAB %nick;
}
