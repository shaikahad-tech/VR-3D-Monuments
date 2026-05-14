const puppeteer = require('puppeteer');

(async () => {
  const browser = await puppeteer.launch({ headless: "new" });
  const page = await browser.newPage();

  page.on('console', msg => {
    if(msg.type() === 'error') console.log('PAGE ERROR:', msg.text());
    else console.log('PAGE LOG:', msg.text());
  });
  
  page.on('pageerror', error => {
    console.log('UNCAUGHT EXCEPTION:', error.message);
  });

  try {
    console.log('Navigating to page...');
    await page.goto('http://127.0.0.1:8080/VirtualMuseum.html', { waitUntil: 'networkidle0', timeout: 5000 });
    console.log('Page loaded!');
    
    // Check if the start button is there
    const btn = await page.$('#startBtn');
    console.log('Start button exists:', !!btn);
    
    if (btn) {
       console.log('Clicking start...');
       await btn.click();
       await new Promise(r => setTimeout(r, 1000));
    }
  } catch(e) {
    console.log('Navigation failed:', e.message);
  }
  await browser.close();
})();
