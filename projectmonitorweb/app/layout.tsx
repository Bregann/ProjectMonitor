import './global.css';
import 'bootstrap/dist/css/bootstrap.min.css';

export default function RootLayout({
    // Layouts must accept a children prop.
    // This will be populated with nested layouts or pages
    children,
  }: {
    children: React.ReactNode;
  }) {

    if(process.env.NODE_ENV === 'development' ){
        process.env.NODE_TLS_REJECT_UNAUTHORIZED = "0";
      }

    return (
      <html lang="en">
        <head>
          <title>Dashboard Monitor</title>
        </head>
        <body>{children}</body>
      </html>
    );
  }