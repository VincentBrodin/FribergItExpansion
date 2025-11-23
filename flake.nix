{
  description = "Dotnet devshell";

  inputs = {
    nixpkgs.url = "github:nixos/nixpkgs/nixos-unstable";
  };

  outputs =
    { self, nixpkgs, ... }:
    let
      system = "x86_64-linux";
      pkgs = import nixpkgs {
        inherit system;
        config = {
          allowUnfree = true;
        };
      };
    in
    {
      devShells.x86_64-linux.default = pkgs.mkShell {
        buildInputs = [
          pkgs.dotnet-sdk_9
          pkgs.dotnet-ef
          pkgs.csharp-ls
          pkgs.vscode-fhs
          pkgs.sqlcmd
          pkgs.nodejs_24
        ];

        shellHook = ''
          alias tw="npx tailwindcss -i ./wwwroot/input.css -o ./wwwroot/output.css --watch"
        '';
      };
    };
}

