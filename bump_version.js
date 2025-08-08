const spawnSync = require("child_process").spawnSync;

class VersionUpgrade {

    _runRelease() {
        this._executeCommand(`git tag -f ${this.version}`);
        this._executeCommand(`git cliff -o CHANGELOG.md`);
        this._executeCommand(`git add CHANGELOG.md`);
        this._executeCommandWithArgs(`git`, ["commit", "-m", `docs: generate changelog for ${this.version}`]);
        this._executeCommand(`git push`);
        this._executeCommand(`git push origin ${this.version} --force`);
    }

    _executeCommand(cmd, options) {
        console.log(`executing: [${cmd}]`)
        let ops = {
            cwd: process.cwd(),
            env: process.env,
            stdio: 'pipe',
            encoding: 'utf-8'
        };
        const INPUT = cmd.split(" "), TOOL = INPUT[0], ARGS = INPUT.slice(1)
        console.log(String(spawnSync(TOOL, ARGS, ops).output));
    }

    _executeCommandWithArgs(cmd, args) {
        console.log(`executing: [${cmd}]`)
        let ops = {
            cwd: process.cwd(),
            env: process.env,
            stdio: 'pipe',
            encoding: 'utf-8'
        };
        console.log(String(spawnSync(cmd, args, ops).output));
    }

    upgrade(argv) {
        console.log("Upgrading version...");
        if (argv.length !== 1) {
            console.log("Only one argument (version) is expected.");
            return;
        }

        this.version = argv[1];
        if (!this._isValidVersion(this.version)) {
            console.error("Invalid version format. Expected 'X.X.X'.");
            return;
        }
        console.log("Bumping version to " + this.version);
        this._runRelease();
    }

    _isValidVersion(version) {
        const versionPattern = /^\d+\.\d+\.\d+(-[a-zA-Z0-9]+)?$/;
        return versionPattern.test(version);
    }
}

new VersionUpgrade().upgrade(process.argv);