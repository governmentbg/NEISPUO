const express = require("express");
const cors = require("cors");
const fs = require("fs");

const { SignPDF } = require("./sign_pdf");

const app = express();
const port = process.env.PORT || 5339;

app.use(cors());
app.use(express.json({ limit: "50mb" }));

app.post("/api/certificate/signPdf", async (req, res) => {
  // expects exactly this request body:
  // {
  //   contents: "<base64>",
  //   options: {
  //     isVisible: true,
  //     pagePosition: "first",
  //   }
  // }
  if (
    typeof req.body.contents !== "string" ||
    req.body.contents.length === 0 ||
    req.body.options.isVisible !== true ||
    req.body.options.pagePosition !== "first"
  ) {
    res.status(400).json({ message: "Invalid request body" });
    return;
  }

  try {
    const pdfBuffer = Buffer.from(req.body.contents, "base64");
    const p12Buffer = fs.readFileSync("certificate.p12");

    const signPDF = new SignPDF(pdfBuffer, p12Buffer, "password");
    const signedPdf = await signPDF.signPDF();

    const base64EncodedPdf = signedPdf.toString("base64");

    res.json({
      contents: base64EncodedPdf,
      thumbprint: "5B80B90A8B63E13095979C9186556010289168DD",
      isError: false,
      message: null,
      messageCode: null,
      additionalInformation: null,
    });
  } catch (err) {
    console.error(err);
    res.status(500).json({
      contents: null,
      thumbprint: null,
      isError: true,
      message: "Не е избран сертификат",
      messageCode: 1,
      additionalInformation: null,
    });
  }
});

app.get("/api/server/version", (req, res) => {
  res.json("1.0.8.0");
});

app.get("/api/server/edition", (req, res) => {
  res.json("NEISPUO");
});

app.get("/api/server/caps", (req, res) => {
  res.json(["CAP_CERTIFICATE"]);
});

app.get("/api/server/settings", (req, res) => {
  res.json([
    { key: "UsePOS", value: "False" },
    { key: "ShowConfirmationDialog", value: "False" },
    { key: "DemoMode", value: "False" },
    { key: "POSPort", value: "" },
    { key: "POSManufacturer", value: "" },
    { key: "UpgradeRequired", value: "False" },
    { key: "AutoStart", value: "True" },
  ]);
});

app.listen(port, () => {
  console.log(`running on port ${port}`);
});
