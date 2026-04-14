const api = (path, opts) => fetch(path, Object.assign({headers:{'Content-Type':'application/json'}}, opts)).then(r=>r.ok? r.json().catch(()=>null) : r.text().then(t=>{throw new Error(t||r.status)}));

let currentUser = null;

document.querySelectorAll('nav button[data-page]').forEach(btn=>btn.addEventListener('click',()=>showPage(btn.dataset.page)));
document.getElementById('btnLogout').addEventListener('click', logout);

async function showPage(page){
  const app = document.getElementById('app');
  if(page==='dashboard'){
    app.innerHTML = `<h2>Dashboard</h2><p>Welcome to HIT Clinic web interface.</p>`;
  } else if(page==='patients'){
    const patients = await api('/api/patients');
    app.innerHTML = `<h2>Patients</h2><table id="tbl"><thead><tr><th>Id</th><th>First</th><th>Last</th><th>StudentId</th><th>Phone</th><th></th></tr></thead><tbody></tbody></table>`;
    const tbody = app.querySelector('tbody');
    patients.forEach(p=>{
      const tr = document.createElement('tr');
      tr.innerHTML = `<td>${p.id}</td><td>${p.firstName}</td><td>${p.lastName}</td><td>${p.studentId||''}</td><td>${p.phoneNumber||''}</td><td><button data-id="${p.id}">Edit</button> <button data-iddel="${p.id}">Del</button></td>`;
      tbody.appendChild(tr);
    });
    tbody.querySelectorAll('button[data-id]').forEach(b=>b.addEventListener('click',e=>editPatient(e.target.dataset.id)));
    tbody.querySelectorAll('button[data-iddel]').forEach(b=>b.addEventListener('click',e=>delPatient(e.target.dataset.iddel)));
    app.insertAdjacentHTML('beforeend', `<h3>Add Patient</h3><form id="frm"><div class="row"><input name="firstName" placeholder="First"/><input name="lastName" placeholder="Last"/></div><div class="row"><input name="studentId" placeholder="Student ID"/><input name="phoneNumber" placeholder="Phone"/></div><div class="row"><input type="date" name="dob"/></div><button type="submit">Add</button></form>`);
    app.querySelector('#frm').addEventListener('submit',async e=>{
      e.preventDefault(); const f=new FormData(e.target); try{const p={firstName:f.get('firstName'), lastName:f.get('lastName'), studentId:f.get('studentId'), phoneNumber:f.get('phoneNumber'), dateOfBirth:f.get('dob')}; await api('/api/patients',{method:'POST',body:JSON.stringify(p)}); alert('Added'); showPage('patients'); }catch(err){alert(err.message)}
    });
  } else if(page==='doctors'){
    const docs = await api('/api/doctors');
    app.innerHTML = `<h2>Doctors</h2><table><thead><tr><th>Id</th><th>Name</th><th>Email</th><th>Spec</th><th></th></tr></thead><tbody>${docs.map(d=>`<tr><td>${d.id}</td><td>${d.name}</td><td>${d.email||''}</td><td>${d.specialization||''}</td><td><button data-id="${d.id}">Edit</button></td></tr>`).join('')}</tbody></table>`;
  } else if(page==='appointments'){
    const appts = await api('/api/appointments');
    const pats = await api('/api/patients');
    const docs = await api('/api/doctors');
    app.innerHTML = `<h2>Appointments</h2><table><thead><tr><th>Id</th><th>Patient</th><th>Doctor</th><th>Date</th><th>Reason</th></tr></thead><tbody>${appts.map(a=>`<tr><td>${a.appointmentId}</td><td>${a.patient?.firstName} ${a.patient?.lastName}</td><td>${a.doctor?.name}</td><td>${new Date(a.date).toLocaleString()}</td><td>${a.reason}</td></tr>`).join('')}</tbody></table>`;
    app.insertAdjacentHTML('beforeend', `<h3>Book</h3><form id="frmA"><div class="row"><select name="patient">${pats.map(p=>`<option value="${p.id}">${p.firstName} ${p.lastName}</option>`)}</select><select name="doctor">${docs.map(d=>`<option value="${d.id}">${d.name}</option>`)}</select></div><div class="row"><input name="date" type="datetime-local"/></div><div class="row"><input name="reason" placeholder="Reason"/></div><button>Book</button></form>`);
    app.querySelector('#frmA').addEventListener('submit',async e=>{e.preventDefault(); const f=new FormData(e.target); try{await api('/api/appointments',{method:'POST',body:JSON.stringify({patient:{id:parseInt(f.get('patient'))},doctor:{id:parseInt(f.get('doctor'))},date:f.get('date'),reason:f.get('reason')})}); alert('Booked'); showPage('appointments'); }catch(err){alert(err.message)}});
  } else if(page==='billing'){
    const bills = await api('/api/bills');
    const pats = await api('/api/patients');
    app.innerHTML = `<h2>Billing</h2><table><thead><tr><th>Id</th><th>Patient</th><th>Amount</th><th>Date</th></tr></thead><tbody>${bills.map(b=>`<tr><td>${b.billId}</td><td>${b.patient?.firstName} ${b.patient?.lastName}</td><td>${b.amount}</td><td>${new Date(b.date).toLocaleDateString()}</td></tr>`).join('')}</tbody></table>`;
    app.insertAdjacentHTML('beforeend', `<h3>New Bill</h3><form id="frmB"><div class="row"><select name="patient">${pats.map(p=>`<option value="${p.id}">${p.firstName} ${p.lastName}</option>`)}</select><input name="amount" placeholder="Amount"/></div><div class="row"><input type="date" name="date"/></div><button>Create</button></form>`);
    app.querySelector('#frmB').addEventListener('submit',async e=>{e.preventDefault(); const f=new FormData(e.target); try{await api('/api/bills',{method:'POST',body:JSON.stringify({patient:{id:parseInt(f.get('patient'))},amount:parseFloat(f.get('amount')),date:f.get('date')})}); alert('Created'); showPage('billing'); }catch(err){alert(err.message)}});
  } else if(page==='doctorPortal'){
    const doc = await api('/api/doctors');
    const patients = await api('/api/patients');
    app.innerHTML = `<h2>Doctor Portal</h2><div id="portal"></div>`;
    const portal = document.getElementById('portal');
    portal.innerHTML = `<h3>Log Illness</h3><form id="frmI"><div class="row"><select name="patient">${patients.map(p=>`<option value="${p.id}">${p.firstName} ${p.lastName}</option>`)}</select><input name="date" type="datetime-local"/></div><div class="row"><input name="diagnosis" placeholder="Diagnosis"/><input name="notes" placeholder="Notes"/></div><button>Log</button></form><h3>Your Records</h3><div id="records"></div>`;
    document.getElementById('frmI').addEventListener('submit',async e=>{e.preventDefault(); const f=new FormData(e.target); try{await api('/api/illness',{method:'POST',body:JSON.stringify({patient:{id:parseInt(f.get('patient'))},doctor:{id:/* server uses current doctor, for web demo supply null */0},date:f.get('date'),diagnosis:f.get('diagnosis'),notes:f.get('notes')})}); alert('Logged'); showPage('doctorPortal'); }catch(err){alert(err.message)}});
    // show records for current doctor: NOTE: demo uses server-side AuthService; call /api/illness/doctor/{id}
    // Here we prompt for doctor id
    let did = prompt('Enter your doctor id for demo (e.g. 1)');
    if(did){ const recs = await api(`/api/illness/doctor/${did}`); const container=document.getElementById('records'); container.innerHTML = `<table><thead><tr><th>Id</th><th>Patient</th><th>Date</th><th>Diagnosis</th><th>Notes</th></tr></thead><tbody>${recs.map(r=>`<tr><td>${r.id}</td><td>${r.patient?.firstName} ${r.patient?.lastName}</td><td>${new Date(r.date).toLocaleString()}</td><td>${r.diagnosis}</td><td>${r.notes}</td></tr>`).join('')}</tbody></table>`; }
  }
}

async function editPatient(id){
  const p = await api(`/api/patients/${id}`);
  const fname = prompt('First name',p.firstName);
  if(!fname) return;
  p.firstName=fname; p.lastName=prompt('Last',p.lastName) || p.lastName; p.studentId=prompt('Student ID',p.studentId) || p.studentId; try{ await api(`/api/patients/${id}`,{method:'PUT',body:JSON.stringify(p)}); alert('Updated'); showPage('patients'); }catch(e){alert(e.message)}
}
async function delPatient(id){ if(!confirm('Delete?')) return; await api(`/api/patients/${id}`,{method:'DELETE'}); alert('Deleted'); showPage('patients'); }

async function logout(){ await api('/api/logout',{method:'POST'}); currentUser=null; document.getElementById('userArea').textContent=''; alert('Logged out'); }

// initial
showPage('dashboard');

// simple login prompt to set user area
(async function(){
  if(confirm('Login as student? OK=student, Cancel=doctor')){
    const id = prompt('Enter student ID (e.g. H240101A)');
    if(id){ try{ const res = await api('/api/login/student',{method:'POST',body:JSON.stringify({identifier:id})}); document.getElementById('userArea').textContent=`Student: ${res.studentId}`; }catch(e){alert(e.message)} }
  } else {
    const em = prompt('Enter doctor email'); if(em){ try{ const res=await api('/api/login/doctor',{method:'POST',body:JSON.stringify({identifier:em})}); document.getElementById('userArea').textContent=`Doctor: ${res.email}`; }catch(e){alert(e.message)} }
  }
})();
