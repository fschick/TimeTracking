export function $localizeId(messageParts: TemplateStringsArray, ...expressions: any[]): string {
  // Create writeable copies
  const messagePartsCopy: any = [...messageParts];
  const messagePartsRawCopy = [...messageParts.raw];

  // Strip trans-unit-id element
  const prefix = messagePartsCopy.shift();
  const prefixRaw = messagePartsRawCopy.shift();
  const transUnitId = expressions.shift();

  // Re-add prefix and replace :TRANSUNITID: with transUnitId.
  messagePartsCopy[0] = prefix + messagePartsCopy[0].replace(':TRANSUNITID:', `:${transUnitId}:`);
  messagePartsRawCopy[0] = prefixRaw + messagePartsRawCopy[0].replace(':TRANSUNITID:', `:@@${transUnitId}:`);

  // Create messageParts object
  Object.defineProperty(messagePartsCopy, 'raw', {value: messagePartsRawCopy});

  // Call original localize function
  return $localize(messagePartsCopy as TemplateStringsArray, ...expressions);
}
