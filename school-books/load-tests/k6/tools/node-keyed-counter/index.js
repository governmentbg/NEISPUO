const express = require('express');
const app = express();
const port = 5500;

const map = new Map();

app.get('/:key', (req, res) => {
  const key = req.params.key;
  let next = 0;
  if (map.has(key)) {
    next = map.get(key);
  }

  map.set(key, next + 1);
  res.status(200).json(next);
});

app.listen(port, () => {
  console.log(`listening on port ${port}`);
});
