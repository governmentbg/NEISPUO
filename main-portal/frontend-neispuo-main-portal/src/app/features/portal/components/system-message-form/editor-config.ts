import { RawEditorSettings } from 'tinymce';

export const SYSTEM_MESSAGE_EDITOR_CONFIG: RawEditorSettings = {
  height: 320,
  menubar: false,
  plugins: [
    'advlist autolink lists link image charmap print preview anchor',
    'searchreplace visualblocks code fullscreen',
    'insertdatetime media table paste code help wordcount'
  ],
  toolbar:
    'undo redo | bold italic underline strikethrough | forecolor backcolor | bullist numlist outdent indent | removeformat |',
  content_style:
    'body { font-family: -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, Helvetica, Arial, sans-serif; font-size: 14px; }',
  placeholder: 'Въведете съдържание...',
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
  indentation: '20px',
  lists_indent_on_tab: true,
  convert_urls: false,
  relative_urls: false,
  remove_script_host: false,
  entity_encoding: 'raw',
  valid_elements: 'p[style],span[style],strong,em,s,u,b,i,ul,ol,li',
  valid_styles: {
    '*': 'color,background-color,text-decoration'
  },
  setup: (editor) => {
    editor.on('init', () => {
      editor.formatter.register('alignleft', {
        block: 'p',
        styles: { textAlign: 'left' }
      });
      editor.formatter.register('aligncenter', {
        block: 'p',
        styles: { textAlign: 'center' }
      });
      editor.formatter.register('alignright', {
        block: 'p',
        styles: { textAlign: 'right' }
      });
      editor.formatter.register('alignjustify', {
        block: 'p',
        styles: { textAlign: 'justify' }
      });
    });
  }
};
