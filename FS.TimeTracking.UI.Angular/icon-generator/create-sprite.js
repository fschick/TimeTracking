const fs = require('fs');
const { exec } = require('child_process');
const source = process.argv[2];
const fileList = `${__dirname}/${source}-files.txt`;
const configFile =  `${__dirname}/${source}-sprite.json`;
const files = fs.readFileSync(fileList,'utf8').replace(/(\r|\n|\r\n)/g, ' ');
const command = `npx svg-sprite --config ${configFile} --log=info`;
exec(`${command} ${files}`).stdout.pipe(process.stdout);