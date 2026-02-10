import { RawEditorSettings } from 'tinymce';

export const EMAIL_TEMPLATE_EDITOR_CONFIG: RawEditorSettings = {
  height: 350,
  menubar: false,
  plugins: [
    'advlist autolink lists link image charmap print preview anchor',
    'searchreplace visualblocks code fullscreen',
    'insertdatetime media table paste code help wordcount'
  ],
  toolbar:
    'undo redo | bold italic underline strikethrough | forecolor backcolor | bullist numlist outdent indent | removeformat',
  content_style:
    'body { font-family: -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, Helvetica, Arial, sans-serif; font-size: 14px; }',
  placeholder: 'Въведете съдържание...',
  contextmenu: 'copy selectall',
  branding: false,
  resize: true,
  statusbar: false,
  formats: {
    bold: { inline: 'strong' },
    italic: { inline: 'em' },
    underline: { inline: 'span', styles: { textDecoration: 'underline' } },
    strikethrough: { inline: 's' }
  },
  style_formats_merge: false,
  lists_indent_on_tab: true,
  removeformat_selector: 'b,strong,em,i,u,h1,h2,h3,h4,h5,h6,p,span,div'
};

export const PLACEHOLDER_INSERTED_STYLES = [
  'all: unset',
  'font-weight: normal',
  'font-style: normal',
  'text-decoration: none',
  'color: #495057 !important',
  'font-family: monospace',
  'font-size: 0.875rem',
  'line-height: 1.5',
  'display: inline-block',
  'padding: 0.15rem 0.2rem',
  'background-color: #e9ecef',
  'border: 1px solid #dee2e6',
  'border-radius: 0.25rem',
  'user-select: none',
  'cursor: default'
].join('; ');
