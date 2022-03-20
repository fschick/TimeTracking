const fs = require('fs');
const { exec } = require('child_process');

const configFile = process.argv[2];
const fileList = process.argv[3];
const destDirectory = process.argv[4];
const files = fs.readFileSync(fileList,'utf8').replace(/(\r|\n|\r\n)/g, ' ');
const command = `npx svg-sprite --config ${configFile} --log=info --dest=${destDirectory}`;

exec(`${command} ${files}`).stdout.pipe(process.stdout);