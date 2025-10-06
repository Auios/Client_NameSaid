import bl from "@lib/Blockland";
import { clientCmdChatMessageArgs } from "@lib/CommonArgs";

bl.new("AudioProfile nameChatSound", {
  fileName: "Add-Ons/Client_NameSaid/nameSound.wav",
  description: "AudioGui",
  preload: true,
});

// Only keep alphanumeric characters for name check, but also check original
const alnum = (str: string) =>
  str
    .split("")
    .filter((c) => {
      const code = c.charCodeAt(0);
      return (
        (code >= 48 && code <= 57) || // 0-9
        (code >= 65 && code <= 90) || // A-Z
        (code >= 97 && code <= 122) // a-z
      );
    })
    .join("");

bl.hook(
  "nameSaid",
  "clientCmdChatMessage",
  "before",
  function (data: clientCmdChatMessageArgs) {
    const name = bl["pref::Player::NetName"] as string;
    const nameSaidSound = bl["pref::Client::nameSaidSound"] as boolean;

    if (!nameSaidSound || !name) return;

    const message = data[7] as string;
    const messageLower = message.toLowerCase();
    const nameLower = name.toLowerCase();
    const nameAlnum = alnum(name);
    const nameAlnumLower = nameAlnum.toLowerCase();

    const needles = [{ original: name, lower: nameLower, len: name.length }];
    if (nameLower !== nameAlnumLower) {
      needles.push({
        original: nameAlnum,
        lower: nameAlnumLower,
        len: nameAlnum.length,
      });
    }

    needles.sort((a, b) => b.len - a.len);

    if (needles.some((needle) => messageLower.includes(needle.lower))) {
      bl.alxPlay("nameChatSound");

      const originalFullMessage = data[3] as string;
      const originalUserMessage = data[7] as string;

      let modifiedUserMessage = "";
      let lastIndex = 0;

      for (let i = 0; i < originalUserMessage.length; ) {
        let foundMatch = false;

        for (const needle of needles) {
          if (messageLower.substring(i).startsWith(needle.lower)) {
            modifiedUserMessage += originalUserMessage.substring(lastIndex, i);
            const originalMatch = originalUserMessage.substring(
              i,
              i + needle.len
            );
            modifiedUserMessage += `<color:1589FF>${originalMatch}<color:ffffff>`;
            i += needle.len;
            lastIndex = i;
            foundMatch = true;
            break; // Found the best (longest) match for this position
          }
        }

        if (!foundMatch) {
          i++;
        }
      }

      if (lastIndex > 0) {
        modifiedUserMessage += originalUserMessage.substring(lastIndex);
        const finalFullMessage = originalFullMessage.replace(
          originalUserMessage,
          modifiedUserMessage
        );
        data[3] = finalFullMessage;
      }
    }
  }
);
