if(!isFunction("luaexec")) {
  echo("[Client_NameSaid] BlockLua not found");
  return;
}

luaexec("./main.lua");
