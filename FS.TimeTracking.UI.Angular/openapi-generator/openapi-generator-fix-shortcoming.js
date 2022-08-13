const {resolve} = require('path');
const {readdir, readFile, writeFile} = require('fs').promises;

const directory = process.argv[2];
console.log('Replace in directory ', directory);

const fixes = [
    // DateTime is already imported by adjusted template.
	{search: /\/\/ @ts-ignore(\r\n|\n|\r)import { DateTime } from 'luxon';/, replace: ""},
	// {search: /cc/g, replace: 'ee'},
];

replaceInDirectory(directory, fixes).then(() => console.log('finished'));

async function replaceInDirectory(directory, replacements) {
    for await (const file of getFiles(directory))
        await replaceInFile(file, replacements);
}

async function* getFiles(directory) {
    const dirEntries = await readdir(directory, {withFileTypes: true});
    for (const entry of dirEntries) {
        const entryName = resolve(directory, entry.name);
        if (entry.isDirectory())
            yield* getFiles(entryName);
        else
            yield entryName;
    }
}

async function replaceInFile(file, replacements) {
	console.log('Replace for file:', file);
    const fileContent = await readFile(file, 'utf8');

    let replacedContent = fileContent;
    for (const replacement of replacements)
        replacedContent = replacedContent.replace(replacement.search, replacement.replace);

    if (replacedContent !== fileContent)
        await writeFile(file, replacedContent, 'utf8');
}