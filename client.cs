new AudioProfile(nameChatSound) {
    fileName = "./nameSound.wav";
    description = AudioGui;
    preload = true;
};

if(!isFunction("luaexec")) {
  echo("[Client_NameSaid] BlockLua not found");
  return;
}

luaexec("./main.lua");
